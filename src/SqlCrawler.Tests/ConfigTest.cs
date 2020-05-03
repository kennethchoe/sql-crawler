using Autofac;
using NUnit.Framework;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Core;
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
            Assert.That(string.IsNullOrEmpty("test.csv"), Is.False);
        }
    }
}
