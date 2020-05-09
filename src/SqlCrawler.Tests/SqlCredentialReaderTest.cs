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
        public void BuildConnectionString()
        {
            var appConfig = new AppConfig {SqlCredentialsFilePath = new [] {"Resources", "sql-credentials-connstr-test.csv"}};

            var scope = BuildCustomScope(appConfig);

            var reader = scope.Resolve<SqlCredentialReader>();
            var infos = reader.Read().ToList();
            Console.WriteLine(JsonConvert.SerializeObject(infos, Formatting.Indented));

            var runner = scope.Resolve<SqlRunner>();
            foreach (var info in infos)
            {
                var actualConnectionString = runner.GetDriver(info).BuildConnectionString(info);
                Assert.AreEqual(info.Description, actualConnectionString);
            }
        }

        [Test]
        public void DuplicateServerIdsShouldRaiseException()
        {
            var appConfig = new AppConfig { SqlCredentialsFilePath = new[] { "Resources", "sql-credentials-dup-server-ids.csv" }};

            var scope = BuildCustomScope(appConfig);

            var reader = scope.Resolve<SqlCredentialReader>();
            Assert.Throws(
                Is.TypeOf<Exception>()
                    .And.Message.Contains("svr1, svr2"),
                delegate
                {
                    // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                    reader.Read().ToList();
                });
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
