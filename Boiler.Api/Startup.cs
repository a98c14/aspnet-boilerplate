using Boiler.Api.Extensions;
using Boiler.Api.Features.Auth.Extensions;
using Boiler.Infrastructure;
using Boiler.Util.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Boiler.Infrastructure.Interfaces;

namespace Boiler.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment m_HostContext;
        private const string CORS_ALL = "All";

        public Startup(IConfiguration config, IHostingEnvironment hostContext)
        {
            Configuration = config;
            m_HostContext = hostContext;
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
            services.AddDistributedCache(m_HostContext, Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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