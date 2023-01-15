using System;
using Microsoft.Extensions.Configuration;

namespace Finbourne.Cache.Component.Configuration
{
    public static class ConfigurationManager
    {
        public static IConfiguration AppSettings { get; }

        static ConfigurationManager()
        {
            AppSettings = new ConfigurationBuilder()
                                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                .AddJsonFile("appsettings.json")
#if !DEBUG
                                .AddJsonFile("appsettings.release.json")
#endif
                                .AddEnvironmentVariables()
                                .Build();
        }
    }
}