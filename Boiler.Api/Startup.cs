using Boiler.Api.Persistence;
using Boiler.Auth.Extensions;
using Boiler.Auth.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Boiler.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private const string CORS_ALL = "All";

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy(CORS_ALL, build => build.AllowAnyHeader()
                                                                                  .AllowAnyOrigin()
                                                                                  .AllowAnyMethod()));

            services.AddDbContext<DataContext>();
            
            services.AddScoped<IAuthContext>(provider => provider.GetService<DataContext>());
            services.AddControllers()
                    .AddAuthControllers();
            services.AddAuth();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(CORS_ALL);
            app.UseAuth();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
