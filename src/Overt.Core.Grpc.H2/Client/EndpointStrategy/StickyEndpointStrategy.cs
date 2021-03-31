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
        private readonly ConcurrentDictionary<string, List<ChannelWrapper>> _channelWrappers = new ConcurrentDictionary<string, List<ChannelWrapper>>();

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

            foreach (var item in _channelWrappers)
            {
                item.Value?.ForEach(channel =>
                {
                    channel.ShutdownAsync();
                });
            }
            _channelWrappers.Clear();
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
        public List<ChannelWrapper> GetChannelWrappers(string serviceName)
        {
            if (_channelWrappers.TryGetValue(serviceName, out List<ChannelWrapper> channels) &&
                channels.Count > 0)
                return channels;

            lock (_lock)
            {
                if (_channelWrappers.TryGetValue(serviceName, out channels) &&
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
        public ChannelWrapper GetChannelWrapper(string serviceName)
        {
            var channels = GetChannelWrappers(serviceName);
            return ServicePollingPolicy.Random(channels);
        }

        /// <summary>
        /// 屏蔽
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="failedCallInvoker"></param>
        public void Revoke(string serviceName, ChannelWrapper channelWrapper)
        {
            lock (_lock)
            {
                if (channelWrapper == null)
                    return;

                // channels
                if (_channelWrappers.TryGetValue(serviceName, out List<ChannelWrapper> channels) &&
                    channels.Any(oo => oo == channelWrapper))
                {
                    channels.Remove(channelWrapper);
                    _channelWrappers.AddOrUpdate(serviceName, channels, (k, v) => channels);
                }

                // add black
                ServiceBlackPolicy.Add(serviceName, channelWrapper.Target);

                channelWrapper.ShutdownAsync();

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
                        foreach (var item in _channelWrappers)
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
        private List<ChannelWrapper> GetSetChannels(string serviceName, bool filterBlack = true)
        {
            if (!_discoveries.TryGetValue(serviceName, out IEndpointDiscovery discovery))
                return null;

            _channelWrappers.TryGetValue(serviceName, out List<ChannelWrapper> channelWrappers);
            channelWrappers ??= new List<ChannelWrapper>();
            var targets = discovery.FindServiceEndpoints(filterBlack);
            if ((targets?.Count ?? 0) <= 0)
            {
                // 如果consul 取不到 暂时直接使用本地缓存的连接（注册中心数据清空的情况--异常）
                _channelWrappers.TryGetValue(serviceName, out channelWrappers);
                return channelWrappers;
            }

            foreach (var target in targets)
            {
                if (channelWrappers.Any(oo => oo.Target == target.target))
                    continue;

                var channel = GrpcChannel.ForAddress($"https://{target.target}", Constants.DefaultChannelOptions);
                var channelWrapper = new ChannelWrapper(target.serviceId, channel);
                channelWrappers.Add(channelWrapper);
            }

            // 移除已经销毁的callInvokers
            var destroyChannels = channelWrappers.Where(oo => !targets.Any(target => target.target == oo.Target)).ToList();
            foreach (var channel in destroyChannels)
            {
                channelWrappers.Remove(channel);
                channel.ShutdownAsync();
            }

            _channelWrappers.AddOrUpdate(serviceName, channelWrappers, (key, value) => channelWrappers);
            return channelWrappers;
        }
        #endregion
    }
}