using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(sqls, Formatting.Indented));

            Assert.That(sqls.Keys.Contains("Server DateTime"), Is.True);
        }
    }
}
