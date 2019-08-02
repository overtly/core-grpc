using Grpc.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Overt.Core.Grpc
{
    internal class StickyEndpointStrategy : IEndpointStrategy
    {
        private readonly object _lock = new object();
        private readonly Timer _timer;
        private readonly ConcurrentDictionary<string, IEndpointDiscovery> _discoveries = new ConcurrentDictionary<string, IEndpointDiscovery>();
        private readonly ConcurrentDictionary<string, List<ServerCallInvoker>> _invokers = new ConcurrentDictionary<string, List<ServerCallInvoker>>();
        private readonly ConcurrentDictionary<string, Channel> _channels = new ConcurrentDictionary<string, Channel>();

        #region 构造函数
        StickyEndpointStrategy()
        {
            _timer = new Timer(ClientTimespan.ResetInterval.TotalSeconds * 1000);
            InitCheckTimer();
        }
        #endregion

        #region 析构函数
        ~StickyEndpointStrategy()
        {
            _timer?.Stop();
            _timer?.Dispose();

            foreach (var item in _channels)
            {
                item.Value?.ShutdownAsync();
            }
            _channels.Clear();
            _invokers.Clear();
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
            _discoveries.AddOrUpdate(serviceDiscovery.ServiceName, serviceDiscovery, (k, v) => serviceDiscovery);
        }

        /// <summary>
        /// 获取callinvoker
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public ServerCallInvoker Get(string serviceName)
        {
            if (_invokers.TryGetValue(serviceName, out List<ServerCallInvoker> callInvokers) &&
                callInvokers?.Count > 0)
                return ServicePollingPlicy.Random(callInvokers);

            lock (_lock)
            {
                if (_invokers.TryGetValue(serviceName, out callInvokers) &&
                    callInvokers?.Count > 0)
                    return ServicePollingPlicy.Random(callInvokers);

                callInvokers = SetCallInvokers(serviceName);
                return ServicePollingPlicy.Random(callInvokers);
            }
        }

        /// <summary>
        /// 屏蔽
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="failedCallInvoker"></param>
        public void Revoke(string serviceName, ServerCallInvoker failedCallInvoker)
        {
            lock (_lock)
            {
                if (failedCallInvoker == null)
                    return;

                // invokers
                var failedChannel = failedCallInvoker.Channel;
                if (!_invokers.TryGetValue(serviceName, out List<ServerCallInvoker> callInvokers) ||
                    callInvokers.All(x => !ReferenceEquals(failedChannel, x.Channel)))
                    return;

                callInvokers.RemoveAt(callInvokers.FindIndex(x => ReferenceEquals(failedChannel, x.Channel)));
                _invokers.AddOrUpdate(serviceName, callInvokers, (key, value) => callInvokers);

                // channels
                if (_channels.TryGetValue(failedChannel.Target, out Channel channel) &&
                    ReferenceEquals(channel, failedChannel))
                {
                    _channels.TryRemove(failedChannel.Target, out failedChannel);
                }

                // add black
                ServiceBlackPlicy.Add(serviceName, failedChannel.Target);

                failedChannel.ShutdownAsync();

                // reinit callinvoker
                if (callInvokers.Count <= 0)
                    SetCallInvokers(serviceName, false);
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

                    foreach (var item in _invokers)
                    {
                        SetCallInvokers(item.Key);
                    }
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
        private List<ServerCallInvoker> SetCallInvokers(string serviceName, bool filterBlack = true)
        {
            if (!_discoveries.TryGetValue(serviceName, out IEndpointDiscovery discovery))
                return null;

            _invokers.TryGetValue(serviceName, out List<ServerCallInvoker> callInvokers);
            callInvokers = callInvokers ?? new List<ServerCallInvoker>();
            var targets = discovery.FindServiceEndpoints(filterBlack);
            if ((targets?.Count ?? 0) <= 0)
            {
                // 如果consul 取不到 暂时直接使用本地缓存的连接（注册中心数据清空的情况--异常）
                _invokers.TryGetValue(serviceName, out callInvokers);
                return callInvokers;
            }

            var channelOptions = new List<ChannelOption>()
            {
                new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
                new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue),
            };
            foreach (var target in targets)
            {
                if (!_channels.TryGetValue(target, out Channel channel))
                {
                    channel = new Channel(target, ChannelCredentials.Insecure, channelOptions);
                    _channels.AddOrUpdate(target, channel, (key, value) => channel);
                }
                if (callInvokers.Any(x => ReferenceEquals(x.Channel, channel)))
                    continue;

                var callInvoker = new ServerCallInvoker(channel);
                callInvokers.Add(callInvoker);
            }

            // 移除已经销毁的callInvokers
            var destroyInvokers = callInvokers.Where(oo => !targets.Contains(oo.Channel.Target)).ToList();
            foreach (var invoker in destroyInvokers)
            {
                _channels.TryRemove(invoker.Channel.Target, out Channel channel);
                callInvokers.Remove(invoker);
            }

            _invokers.AddOrUpdate(serviceName, callInvokers, (key, value) => callInvokers);
            return callInvokers;
        }
        #endregion
    }
}