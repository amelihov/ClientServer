using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebHostHelpers
{
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
