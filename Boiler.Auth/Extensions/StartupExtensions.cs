using Boiler.Auth.Middleware;
using Boiler.Auth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Boiler.Auth.Extensions
{
    public static class StartupExtensions
    {
        public static void AddAuth(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
        }

        public static void AddAuthControllers(this IMvcBuilder builder)
        {
            builder.AddApplicationPart(typeof(Controllers.AuthController).Assembly);
        }

        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }
    }
}
