using System;
using System.IO;
using System.Reflection;
using Chat.Server.Domain;
using Chat.Server.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat.Server.Host
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

                var builder = CreateWebHostBuilder<ServerProcessing>(configuration)
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
            services.AddHostedService<ChatServerService>();
            services.AddSingleton<ServerProcessing>();
            services.AddMvc();
            services.AddWebSocketManager();
        }

        public static IWebHostBuilder CreateWebHostBuilder<TStartup>(IConfigurationRoot configuration)
            where TStartup : class
        {
            // https://docs.microsoft.com/ru-ru/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.1
            var builder = WebHost.CreateDefaultBuilder();
            builder.UseStartup<ChatServerStartup>();
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

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}

    