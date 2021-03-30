using Grpc.Net.Client;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// IPEndPoint
    /// </summary>
    internal class IPEndpointStrategy : IEndpointStrategy
    {
        #region Constructor
        private readonly object _lock = new object();
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, IEndpointDiscovery> _discoveries = new ConcurrentDictionary<string, IEndpointDiscovery>();
        private readonly ConcurrentDictionary<string, List<ChannelWrapper>> _channelWrappers = new ConcurrentDictionary<string, List<ChannelWrapper>>();

        IPEndpointStrategy()
        {
            _timer = new Timer(ClientTimespan.ResetInterval.TotalSeconds * 1000);
            InitCheckTimer();
        }
        #endregion

        #region Destructor
        ~IPEndpointStrategy()
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
        private static IPEndpointStrategy _ipEndpintStrategy;
        public static IPEndpointStrategy Instance
        {
            get
            {
                if (_ipEndpintStrategy != null)
                    return _ipEndpintStrategy;
                lock (_instanceLocker)
                {
                    if (_ipEndpintStrategy != null)
                        return _ipEndpintStrategy;

                    _ipEndpintStrategy = new IPEndpointStrategy();
                    return _ipEndpintStrategy;
                }
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 添加ServiceDiscovery
        /// </summary>
        /// <param name="discovery"></param>
        public void AddServiceDiscovery(IEndpointDiscovery discovery)
        {
            _discoveries.AddOrUpdate(discovery.ServiceName, discovery, (k, v) => discovery);
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
        /// 获取
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
        public void Revoke(string serviceName, ChannelWrapper channel)
        {
            lock (_lock)
            {
                if (channel == null)
                    return;

                // channels
                if (_channelWrappers.TryGetValue(serviceName, out List<ChannelWrapper> channels) &&
                    channels.Any(oo => oo == channel))
                {
                    channels.Remove(channel);
                    _channelWrappers.AddOrUpdate(serviceName, channels, (k, v) => channels);
                }

                // add black
                ServiceBlackPolicy.Add(serviceName, channel.Channel.Target);

                channel.ShutdownAsync();

                // reinit callinvoker
                if (channels.Count <= 0)
                    GetSetChannels(serviceName, false);
            }
        }

        /// <summary>
        /// 初始化检查Timer
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

            _channelWrappers.TryGetValue(serviceName, out List<ChannelWrapper> channels);
            channels ??= new List<ChannelWrapper>();
            var targets = discovery.FindServiceEndpoints(filterBlack);
            if ((targets?.Count ?? 0) <= 0)
            {
                // 如果consul 取不到 暂时直接使用本地缓存的连接（注册中心数据清空的情况--异常）
                _channelWrappers.TryGetValue(serviceName, out channels);
                return channels;
            }

            foreach (var target in targets)
            {
                if (channels.Any(oo => oo.Target == target.target))
                    continue;

                var channel = GrpcChannel.ForAddress($"https://{target.target}", Constants.DefaultChannelOptions);
                var channelWrapper = new ChannelWrapper(target.serviceId, channel);
                channels.Add(channelWrapper);
            }

            // 移除已经销毁的callInvokers
            var destroyChannels = channels.Where(oo => !targets.Any(target => target.target == oo.Target)).ToList();
            foreach (var channel in destroyChannels)
            {
                channels.Remove(channel);
                channel.ShutdownAsync();
            }

            _channelWrappers.AddOrUpdate(serviceName, channels, (key, value) => channels);
            return channels;
        }
        #endregion
    }
}