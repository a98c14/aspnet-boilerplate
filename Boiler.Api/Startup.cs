using Boiler.Api.Persistence;
using Boiler.Auth.Extensions;
using Boiler.Auth.Helpers;
using Boiler.Auth.Interfaces;
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
            services.Configure<AuthSettings>(Configuration.GetSection("AuthSettings"));
            services.AddScoped<IAuthContext>(provider => provider.GetService<DataContext>());
            services.AddControllers()
                    .AddAuthControllers();
            services.AddAuth();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if(env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Boiler API"));
            }

            app.UseRouting();
            app.UseCors(CORS_ALL);
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseAuth();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
