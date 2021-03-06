using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boiler.Api.Extensions
{
    public static class StartupExtensions
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
                    System.Array.Empty<string>() }
                };
                c.SwaggerDoc("v1", info);
                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(requirements);
            });
        }
    }
}
