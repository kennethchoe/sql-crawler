using Autofac;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Sqlite;
using SqlCrawler.Web.IoC;

namespace SqlCrawler.Tests
{
    [SetUpFixture]
    class TestBootstrapper
    {
        private static IContainer _container;
        public static ILifetimeScope Scope => _container.BeginLifetimeScope();

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.test.json", optional:false)
                .Build();
            return config;
        }

        [OneTimeSetUp]
        public void Bootstrap()
        {
            _container = GetContainerBuilder().Build();

            var dbUpService = Scope.Resolve<DbUpService>();
            dbUpService.Upgrade();
            var sourceReader = Scope.Resolve<SqlQueryReader>();
            sourceReader.Reload();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            var sqlQueryReader = _container.Resolve<SqlQueryReader>();
            sqlQueryReader.ClearCache();
        }

        public static ContainerBuilder GetContainerBuilder()
        {
            var config = InitConfiguration();
            var builder = new ContainerBuilder();
            builder.RegisterModule(new WebAppModule(config));
            builder.RegisterModule(new TestModule());
            return builder;
        }
    }

    internal class TestModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = new[]
            {
                GetType().Assembly
            };

            builder.RegisterAssemblyTypes(assemblies);
        }
    }
}
