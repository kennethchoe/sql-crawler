using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class SqlCredentialReaderTest
    {
        [Test]
        public void ReadContent()
        {
            var appConfig = new AppConfig {SqlCredentialsFilePath = "sql-credentials-test2.csv"};

            // todo: how do i replace IAppConfig only on the scope? do i do it on container?
            var builder = TestBootstrapper.GetContainerBuilder();
            builder.RegisterInstance(appConfig).As<IAppConfig>();
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();

            var reader = scope.Resolve<SqlCredentialReader>();
            var infos = reader.Read().ToList();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(infos, Formatting.Indented));

            foreach (var info in infos)
            {
                Assert.AreEqual(info.Description, info.ToConnectionString());
            }
        }
    }
}
