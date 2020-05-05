using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class SqlCredentialReaderTest
    {
        [Test]
        public void ToConnectionString()
        {
            var appConfig = new AppConfig {SqlCredentialsFilePath = new [] {"sql-credentials-connstr-test.csv"}};

            var scope = BuildCustomScope(appConfig);

            var reader = scope.Resolve<SqlCredentialReader>();
            var infos = reader.Read().ToList();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(infos, Formatting.Indented));

            foreach (var info in infos)
            {
                Assert.AreEqual(info.Description, info.ToConnectionString(null));
            }
        }

        [Test]
        public void DuplicateServerIdsShouldRaiseException()
        {
            var appConfig = new AppConfig { SqlCredentialsFilePath = new[] { "sql-credentials-dup-server-ids.csv" }};

            var scope = BuildCustomScope(appConfig);

            var reader = scope.Resolve<SqlCredentialReader>();
            Assert.Throws(
                Is.TypeOf<SqlCredentialsException>()
                    .And.Message.Contains("svr1, svr2"),
                delegate { reader.Read().ToList(); });
        }

        private ILifetimeScope BuildCustomScope(AppConfig appConfig)
        {
            // todo: how do i replace IAppConfig only on the scope? do i do it on container?
            var builder = TestBootstrapper.GetContainerBuilder();
            builder.RegisterInstance(appConfig).As<IAppConfig>();
            var container = builder.Build();
            var scope = container.BeginLifetimeScope();
            return scope;
        }
    }
}
