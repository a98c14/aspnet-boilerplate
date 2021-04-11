using Boiler.Api.Extensions;
using Boiler.Auth.Interfaces;
using Boiler.Core.Auth.Extensions;
using Boiler.Infrastructure;
using Boiler.Util.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            services.AddControllers();
            services.AddAuth(Configuration);
            services.AddSwagger();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Boiler API");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseRouting();
            app.UseCors(CORS_ALL);
            app.UseMiddleware<ResponseHeaderMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseAuth();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
