using Finbourne.Cache.Component;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
                       services.AddSingleton<IFinbourneCacheService, FinbourneCacheService>();

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
