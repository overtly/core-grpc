using Grpc.Net.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Overt.Core.Grpc.H2
{
    internal class StickyEndpointStrategy : IEndpointStrategy
    {
        #region Constructor
        private readonly object _lock = new object();
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, IEndpointDiscovery> _discoveries = new ConcurrentDictionary<string, IEndpointDiscovery>();
        private readonly ConcurrentDictionary<string, List<GrpcChannel>> _channels = new ConcurrentDictionary<string, List<GrpcChannel>>();

        StickyEndpointStrategy()
        {
            _timer = new Timer(ClientTimespan.ResetInterval.TotalSeconds * 1000);
            InitCheckTimer();
        }
        #endregion

        #region Destructor
        ~StickyEndpointStrategy()
        {
            _timer?.Stop();
            _timer?.Dispose();

            foreach (var item in _channels)
            {
                item.Value?.ForEach(channel =>
                {
                    channel.ShutdownAsync();
                });
            }
            _channels.Clear();
        }
        #endregion

        #region Instance
        private readonly static object _instanceLocker = new object();
        private static StickyEndpointStrategy _stickyEndpintStrategy;
        public static StickyEndpointStrategy Instance
        {
            get
            {
                if (_stickyEndpintStrategy != null)
                    return _stickyEndpintStrategy;
                lock (_instanceLocker)
                {
                    if (_stickyEndpintStrategy != null)
                        return _stickyEndpintStrategy;

                    _stickyEndpintStrategy = new StickyEndpointStrategy();
                    return _stickyEndpintStrategy;
                }
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 添加ServiceDiscovery
        /// </summary>
        /// <param name="serviceDiscovery"></param>
        public void AddServiceDiscovery(IEndpointDiscovery serviceDiscovery)
        {
            if (serviceDiscovery == null)
                return;

            serviceDiscovery.Watched = () => GetSetChannels(serviceDiscovery.ServiceName, false);
            _discoveries.AddOrUpdate(serviceDiscovery.ServiceName, serviceDiscovery, (k, v) => serviceDiscovery);
        }

        /// <summary>
        /// 获取所有
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public List<GrpcChannel> GetChannels(string serviceName)
        {
            if (_channels.TryGetValue(serviceName, out List<GrpcChannel> channels) &&
                channels.Count > 0)
                return channels;

            lock (_lock)
            {
                if (_channels.TryGetValue(serviceName, out channels) &&
                    channels.Count > 0)
                    return channels;

                channels = GetSetChannels(serviceName);
                if ((channels?.Count ?? 0) <= 0 && ServiceBlackPolicy.Exist(serviceName))
                    channels = GetSetChannels(serviceName, false);

                return channels;
            }
        }

        /// <summary>
        /// 获取callinvoker
        /// 随机获取
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public GrpcChannel GetChannel(string serviceName)
        {
            var channels = GetChannels(serviceName);
            return ServicePollingPolicy.Random(channels);
        }

        /// <summary>
        /// 屏蔽
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="failedCallInvoker"></param>
        public void Revoke(string serviceName, GrpcChannel channel)
        {
            lock (_lock)
            {
                if (channel == null)
                    return;

                // channels
                if (_channels.TryGetValue(serviceName, out List<GrpcChannel> channels) &&
                    channels.Any(oo => oo == channel))
                {
                    channels.Remove(channel);
                    _channels.AddOrUpdate(serviceName, channels, (k, v) => channels);
                }

                // add black
                ServiceBlackPolicy.Add(serviceName, channel.Target);

                channel.ShutdownAsync();

                // reinit callinvoker
                if (channels.Count <= 0)
                    GetSetChannels(serviceName, false);
            }
        }

        /// <summary>
        /// 初始化Timer
        /// </summary>
        public void InitCheckTimer()
        {
            _timer.Elapsed += (sender, e) =>
            {
                lock (_lock)
                {
                    _timer.Stop();

                    try
                    {
                        foreach (var item in _channels)
                        {
                            GetSetChannels(item.Key);
                        }
                    }
                    catch { }

                    _timer.Start();
                }
            };
            _timer.Start();
        }
        #endregion

        #region Private Method
        /// <summary>
        /// 初始化callinvoker
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="filterBlack">过滤黑名单 default true</param>
        /// <returns></returns>
        private List<GrpcChannel> GetSetChannels(string serviceName, bool filterBlack = true)
        {
            if (!_discoveries.TryGetValue(serviceName, out IEndpointDiscovery discovery))
                return null;

            _channels.TryGetValue(serviceName, out List<GrpcChannel> channels);
            channels ??= new List<GrpcChannel>();
            var targets = discovery.FindServiceEndpoints(filterBlack);
            if ((targets?.Count ?? 0) <= 0)
            {
                // 如果consul 取不到 暂时直接使用本地缓存的连接（注册中心数据清空的情况--异常）
                _channels.TryGetValue(serviceName, out channels);
                return channels;
            }

            foreach (var target in targets)
            {
                if (channels.Any(oo => oo.Target == target.target))
                    continue;

                var channel = GrpcChannel.ForAddress(target.target, Constants.DefaultChannelOptions);
                channels.Add(channel);
            }

            // 移除已经销毁的callInvokers
            var destroyChannels = channels.Where(oo => !targets.Any(target => target.target == oo.Target)).ToList();
            foreach (var channel in destroyChannels)
            {
                channels.Remove(channel);
                channel.ShutdownAsync();
            }

            _channels.AddOrUpdate(serviceName, channels, (key, value) => channels);
            return channels;
        }
        #endregion
    }
}