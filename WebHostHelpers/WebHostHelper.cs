using System;
using System.Runtime;
using System.Threading;
using Infrastructure;
using LoggingExtensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace WebHostHelpers
{ 
    public static class WebHostHelper
    {
        public static int StartWebHost<TStartup>(string[] args, 
            string serviceName,
            Action<WebHostSettings>? configure = null)
            where TStartup : class
        {
            var settings = new WebHostSettings();
            configure?.Invoke(settings);
            
            try
            {
                Console.WriteLine("Starting service...");
                var webHost = CreateWebHostBuilder<TStartup>(settings.ExtraConfiguration, settings.KestrelSettings).Build();
                webHost.Run();

                Console.WriteLine("Service stopped.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred while starting service. {ex}");
                return -1;
            }
        }

        private static IConfiguration GetInitialConfig(string[] args)
        {
            var initialConfigBuilder = new ConfigurationBuilder()
                .AddGenericConfig()
                .AddEnvironmentVariables("ASPNETCORE_");

            if (args != null)
                initialConfigBuilder.AddCommandLine(args);

            var initialConfig = (IConfiguration)initialConfigBuilder.Build();
            return initialConfig;
        }

        private static IWebHostBuilder CreateWebHostBuilder<TStartup>(
            IConfiguration? extraConfiguration,
            KestrelSettings kestrelSettings)
            where TStartup : class
        {
            //https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
            var builder = WebHost.CreateDefaultBuilder();
            builder.UseStartup<TStartup>();
            builder.ConfigureKestrel(options =>
            {
                options.AllowSynchronousIO = kestrelSettings.AllowSynchronousIO;
            });
            builder.ConfigureAppConfiguration((_, config) =>
            {
                if (extraConfiguration != null)
                    config.AddConfiguration(extraConfiguration); //overrides appsettings
                config.AddGenericConfig();
                config.AddEnvironmentVariables(); // generic.json rewrites values from configmap-settings, so we apply them again
            });

            return builder;
        }
    }
}