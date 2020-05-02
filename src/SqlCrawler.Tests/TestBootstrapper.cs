using Autofac;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using SqlCrawler.Web.IoC;

namespace SqlCrawler.Tests
{
    [SetUpFixture]
    class TestBootstrapper
    {
        private static IContainer _container;
        public static ILifetimeScope Scope => _container.BeginLifetimeScope();

        public IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.test.json", optional:false)
                .Build();
            return config;
        }

        [OneTimeSetUp]
        public void Bootstrap()
        {
            var config = InitConfiguration();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new WebAppModule(config));
            builder.RegisterModule(new TestModule());
            _container = builder.Build();
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
