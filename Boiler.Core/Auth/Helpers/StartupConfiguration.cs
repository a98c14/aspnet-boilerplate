using Boiler.Core.Auth.Helpers;
using Boiler.Core.Auth;
using Boiler.Core.Auth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Core.Auth.Extensions
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
