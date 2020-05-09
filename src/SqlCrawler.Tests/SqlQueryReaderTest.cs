using System;
using System.Linq;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class SqlQueryReaderTest
    {
        [Test]
        public void Read()
        {
            var reader = TestBootstrapper.Scope.Resolve<SqlQueryReader>();
            var sqls = reader.Read();
            Console.WriteLine(JsonConvert.SerializeObject(sqls, Formatting.Indented));

            Assert.That(sqls.Any(x => x.Name == "1. Parameterized Sqls"), Is.True);
        }

        [TestCase("group1\\name1.sql", "group1", "name1")]
        [TestCase("group1\\group2\\name1.sql", "group1/group2", "name1")]
        [TestCase("group1/name1.sql", "group1", "name1")]
        [TestCase("group1/group2/name1.sql", "group1/group2", "name1")]
        public void Parse(string relativePath, string group, string name)
        {
            var reader = TestBootstrapper.Scope.Resolve<SqlQueryReader>();
            var result = reader.Parse(relativePath);
            Assert.AreEqual(group, result.Scope);
            Assert.AreEqual(name, result.Name);
        }
    }
}
