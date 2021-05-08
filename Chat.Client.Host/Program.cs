using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat.Client.Host
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();

            if (args != null)
                configurationBuilder.AddCommandLine(args);

            var configuration = configurationBuilder.Build();

            try
            {
                Console.WriteLine("Starting service...");

                var builder = CreateWebHostBuilder<ClientProcessing>(configuration)
                    .ConfigureServices(ConfigureServicesBuilder)
                    .Build();

                builder.Run();

                Console.WriteLine("Service stopped.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}Exception occured while starting service.");
                return -1;
            }
        }

        private static void ConfigureServicesBuilder(IServiceCollection services)
        {
            services.AddHostedService<ChatClientService>();
            services.AddSingleton<ClientProcessing>();
        }

        public static IWebHostBuilder CreateWebHostBuilder<TStartup>(IConfigurationRoot configuration)
            where TStartup : class
        {
            var builder = WebHost.CreateDefaultBuilder();
            builder.UseStartup<ChatClientStartup>();
            builder.ConfigureAppConfiguration((context, config) => { config.AddConfiguration(configuration); });
            builder.UseDefaultServiceProvider((context, options) => options.ValidateScopes = context.HostingEnvironment.IsDevelopment());
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.UseConfiguration(configuration);
            builder.ConfigureServices((context, services) =>
            {
                services.Configure<KestrelServerOptions>(context.Configuration.GetSection("Kestrel"));
                services.AddOptions();
            });
            builder.UseKestrel((context, options) => options.Configure(context.Configuration.GetSection("Kestrel")));

            return builder;
        }
    }
}
