using System;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Chat.Server.Domain;
using Chat.Server.Infrastructure;
using Chat.Server.Storages;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat.Server.Host
{

    public class ChatServerStartup : ChatServerStartup<ChatServerService>
    {
        public ChatServerStartup(IConfiguration configuration) : base(configuration)
        {
        }
    }

    public class ChatServerStartup<T> : StartupBase
    {
        public ChatServerStartup(IConfiguration configuration)
            : base(configuration) { }

        protected override void ConfigureServicesImpl(IServiceCollection services)
        {
            services.AddTransient<IMessageProcessor, MessageProcessor>();
            services.AddSingleton<ITcpServer, TcpServer>();
            services.AddSingleton<IClientStorage, ClientStorage>();
            services.AddSingleton<IMessageStorage, MessageStorage>();
            services.AddSingleton<IPublisher, Publisher>();
            services.AddSingleton<IServerCommandProcessor, ServerCommandProcessor>();
            services.AddSingleton<WebSocketMessageHandler>();
            services.Configure<TcpSettings>(config => Configuration.GetSection("TcpSettings").Bind(config));
            
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
            var handler = serviceProvider.GetService<WebSocketMessageHandler>();

            app.UseWebSockets();
            app.Map("/ws", (_app) => _app.UseMiddleware<Domain.WebSocketServer>(handler));
        }
    }

    public abstract class StartupBase
    {
        protected StartupBase(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesImpl(services);
            AfterConfigureServices(services);
        }

        protected abstract void ConfigureServicesImpl(IServiceCollection services);
        protected virtual void AfterConfigureServices(IServiceCollection services) { }

    }
}
