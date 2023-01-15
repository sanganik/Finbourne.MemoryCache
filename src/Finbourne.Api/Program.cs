using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using Finbourne.Cache.Component;

namespace Finbourne.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureServices((httpContext, services) =>
                   {

                       // Singleton
                       services.AddSingleton<IFinbourneMemoryCache, FinbourneMemoryCache>();

                       // setup swagger
                       services.AddSwaggerGen(c =>
                       {
                           c.SwaggerDoc("v1", new OpenApiInfo { Title = "Finbourne memory cache component api", Version = "v1" });
                       });

                       services.AddMvcCore()
                               .AddFormatterMappings()
                               .AddApiExplorer()
                               .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
                   });
    }
}
