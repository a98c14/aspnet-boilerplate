using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Core.Live
{
    public static class StartupConfiguration
    {
        public static void AddBoilerLive(this IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}
