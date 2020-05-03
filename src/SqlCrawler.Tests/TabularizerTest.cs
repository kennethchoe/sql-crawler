using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Sqlite;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class TabularizerTest
    {
        [Test]
        public void Process()
        {
            var records = new List<ResultRecord>
            {
                new ResultRecord
                {
                    ServerId = "a",
                    SessionRowId = 1,
                    DataJson = "[{\"DateTime\":\"2020-05-03T13:38:28.467\"}]"
                }
            };

            var svc = new Tabularizer();
            var result = svc.Process(records);
            Console.WriteLine(JsonConvert.SerializeObject(result));
            var kv = (IDictionary<string, object>) result.Single();
            CollectionAssert.AreEquivalent(new [] { "ServerId", "DateTime"}, kv.Keys);
        }
    }
}
