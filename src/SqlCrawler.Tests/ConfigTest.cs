using Autofac;
using NUnit.Framework;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Tests
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void GetAppConfig()
        {
            var config = TestBootstrapper.Scope.Resolve<IAppConfig>();
            Assert.AreEqual(1, config.CommandTimeoutInSeconds);
        }
    }
}
