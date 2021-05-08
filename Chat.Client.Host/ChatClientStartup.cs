using Chat.Client.Infrastructure;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat.Client.Host
{
    public class ChatClientStartup : ChatClientStartup<ChatClientService>
    {
        public ChatClientStartup(IConfiguration configuration) : base(configuration)
        {
        }
    }

    public class ChatClientStartup<T> : StartupBase
    {
        public ChatClientStartup(IConfiguration configuration)
            : base(configuration) { }

        protected override void ConfigureServicesImpl(IServiceCollection services)
        {
            services.AddTransient<ITcpChatClient, TcpChatClient>();
            services.AddTransient<ITcpMessagesStreamParser, TcpMessagesStreamParser>();
            services.AddTransient<IClientProcessing, ClientProcessing>();
            services.AddSingleton<IPublisher, Publisher>();
            services.Configure<TcpSettings>(config => Configuration.GetSection("TcpSettings").Bind(config));
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {

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
