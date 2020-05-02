using Autofac;
using NUnit.Framework;
using SqlCrawler.Web;

namespace SqlCrawler.Tests
{
    [TestFixture]
    public class ConfigTest
    {
        [Test]
        public void GetAppConfig()
        {
            var config = TestBootstrapper.Scope.Resolve<IAppConfig>();
            Assert.AreEqual("test.csv", config.SqlCredentialsFilePath);
        }
    }
}
