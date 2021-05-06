using Boiler.Api.Features.Auth.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Api.Features.Auth.Extensions
{
    public static class StartupConfiguration
    {
        public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthSettings>(x => configuration.GetSection("AuthSettings"));
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
