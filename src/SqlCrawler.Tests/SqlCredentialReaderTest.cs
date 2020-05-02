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
    class SqlCredentialReaderTest
    {
        [Test]
        public void ReadContent()
        {
            var reader = TestBootstrapper.Scope.Resolve<SqlCredentialReader>();
            var infos = reader.Read().ToList();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(infos, Formatting.Indented));

            foreach (var info in infos)
            {
                Assert.AreEqual(info.Description, info.ToConnectionString());
            }
        }
    }
}
