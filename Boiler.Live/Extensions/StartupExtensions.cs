using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Live.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBoilerLive(this IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}
