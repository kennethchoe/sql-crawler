using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using SqlCrawler.Backend.Core;

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

            builder.Register(x => new SqliteConnection("Data Source=" + Path.Combine(appConfig.SqliteDataPath))).As<SqliteConnection>();

            var assemblies = new[]
            {
                GetType().Assembly,
                Assembly.GetAssembly(typeof(Backend.Dll))
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