#if !ASP_NET_CORE
using System.Configuration;
#else
using Microsoft.Extensions.Configuration;
#endif
using System;
using System.IO;

namespace Sodao.Core.Grpc
{
    /// <summary>
    /// 配置文件读取
    /// </summary>
    internal class ConfigHelper
    {
        /// <summary>
        /// 获取Server配置对象
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static T Get<T>(string sectionName, string configPath = "") where T :
#if !ASP_NET_CORE
            ConfigurationSection
#else
            class, new()
#endif
        {
            T section = null;
#if !ASP_NET_CORE
            if (string.IsNullOrEmpty(configPath))
                section = ConfigurationManager.GetSection(sectionName) as T;

            else
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath);
                if (!File.Exists(configPath))
                    throw new ConfigurationErrorsException($"sodao: when resolve configpath, configpath file is not exist...[{configPath}]");

                section = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configPath
                }, ConfigurationUserLevel.None).GetSection(sectionName) as T;
            }
#else
            if (string.IsNullOrEmpty(configPath) || !configPath.EndsWith(".json"))
                configPath = "appsettings.json";

            configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configPath);
            if (!File.Exists(configPath))
                throw new Exception($"sodao: when resolve configpath, configpath file is not exist... [{configPath}]");

            section = new T();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(configPath)
                .AddEnvironmentVariables()
                .Build();

            configuration.GetSection(sectionName).Bind(section);
#endif
            return section;
        }
    }
}
