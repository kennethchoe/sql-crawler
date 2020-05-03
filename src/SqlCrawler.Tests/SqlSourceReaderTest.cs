using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class SqlSourceReaderTest
    {
        [Test]
        public void Read()
        {
            var reader = TestBootstrapper.Scope.Resolve<SqlSourceReader>();
            var sqls = reader.Read();
            Console.WriteLine(JsonConvert.SerializeObject(sqls, Formatting.Indented));

            Assert.That(sqls.Any(x => x.Name == "Server DateTime"), Is.True);
        }
    }
}
