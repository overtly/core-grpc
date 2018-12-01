#if NET45 || NET46 || NET47
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
#endif

namespace Sodao.Core.Grpc
{
#if NET45 || NET46 || NET47
    /// <summary>
    /// GrpcEndpoint
    /// </summary>
    [ConfigurationCollection(typeof(GrpcEndpointElement), AddItemName = "endpoint")]
    public class GrpcEndpointElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 新节点
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new GrpcEndpointElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as GrpcEndpointElement).ToString();
        }

        /// <summary>
        /// 获取指定位置的对象。
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public GrpcEndpointElement this[int i]
        {
            get { return BaseGet(i) as GrpcEndpointElement; }
        }

        /// <summary>
        /// to array
        /// </summary>
        /// <returns></returns>
        public Tuple<string, int>[] ToArray()
        {
            var endpoints = new Tuple<string, int>[base.Count];
            for (int i = 0, l = this.Count; i < l; i++)
            {
                var child = this[i];
                endpoints[i] = new Tuple<string, int>(child.Host, child.Port);
            }
            return endpoints;
        }

        public List<Tuple<string, int>> ToList()
        {
            return this.ToArray().ToList();
        }
    }
#endif
}
