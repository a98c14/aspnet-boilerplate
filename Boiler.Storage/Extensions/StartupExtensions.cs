using Boiler.Storage.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Storage.Extensions
{
    public static class StartupExtensions
    {
        public static void AddStorageServices(this IServiceCollection services)
        {
            services.AddScoped<BlobStorageService>();
        }
    }
}
