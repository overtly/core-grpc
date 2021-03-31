using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Overt.Core.Grpc.H2
{
    /// <summary>
    /// 配置文件读取
    /// </summary>
    internal class ConfigBuilder
    {
        public static Action<IConfigurationBuilder> ConfigureDelegate;

        /// <summary>
        /// 获取Server配置对象
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static T Build<T>(string sectionName, string configPath = "") where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(configPath) || !configPath.EndsWith(".json"))
                configPath = "appsettings.json";

            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath);
            if (!File.Exists(configPath))
                throw new Exception($"overt: when resolve configpath, configpath file is not exist... [{configPath}]");

            var section = new T();
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(configPath)
                .AddEnvironmentVariables();
            ConfigureDelegate?.Invoke(builder);
            var configuration = builder.Build();
            configuration.GetSection(sectionName).Bind(section);
            return section;
        }
    }
}
