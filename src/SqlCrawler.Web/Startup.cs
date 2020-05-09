using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SqlCrawler.Backend.Persistence;
using SqlCrawler.Web.IoC;

namespace SqlCrawler.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public ILifetimeScope AutofacContainer { get; private set; }

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddResponseCompression();
            services.AddOpenApiDocument(config => { config.PostProcess = d => { d.Info.Title = "sql-crawler"; }; });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new WebAppModule(_config));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbUpService dbUpService)
        {
            AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            dbUpService.Upgrade();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseDefaultFiles();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();

            app.UseOpenApi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
