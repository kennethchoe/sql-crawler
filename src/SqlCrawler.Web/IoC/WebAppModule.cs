using System;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace SqlCrawler.Web.IoC
{
    public class WebAppModule : Autofac.Module
    {
        private readonly IConfiguration _config;

        public WebAppModule(IConfiguration config)
        {
            _config = config;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var appConfig = new AppConfig();
            _config.GetSection("App").Bind(appConfig);
            builder.RegisterInstance(appConfig).As<IAppConfig>();

            var assemblies = new[]
            {
                GetType().Assembly
            };

            builder.RegisterAssemblyTypes(assemblies);

            var enumerableInjectionTypes = new Type[]
            {
            }.ToList();

            enumerableInjectionTypes.ForEach(x =>
                    builder.RegisterAssemblyTypes(assemblies)
                        .Where(t => x.IsAssignableFrom(t))
                        .AsImplementedInterfaces()
                );
        }
    }
}