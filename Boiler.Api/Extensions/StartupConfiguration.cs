using Boiler.Auth.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace Boiler.Api.Extensions
{
    public static class StartupConfiguration
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var info = new OpenApiInfo
                {
                    Title = "Boiler Api",
                    Version = "v1",
                };
                var scheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert token below ( Don't forget to put `Bearer` before the token )",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                };
                var requirements = new OpenApiSecurityRequirement()
                {
                    { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>() }
                };
                c.SwaggerDoc("v1", info);
                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(requirements);
            });
        }

        public static void UseAuth(this IApplicationBuilder app)
        {
            app.UseMiddleware<JwtMiddleware>();
        }

        public static void AddDistributedCache(this IServiceCollection services, IHostingEnvironment m_HostContext, IConfiguration Configuration)
        {
            if (m_HostContext.IsDevelopment())
                services.AddDistributedMemoryCache();
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    // Change it according to Redis hosting location.
                    options.Configuration = Configuration["RedisLocation"];
                    options.InstanceName = "SampleInstance";
                });
            }
        }
    }
}