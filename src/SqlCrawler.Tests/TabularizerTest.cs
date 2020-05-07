using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
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

            var svc = TestBootstrapper.Scope.Resolve<Tabularizer>();

            var result = svc.Process(records);
            Console.WriteLine(JsonConvert.SerializeObject(result));
            var kv = (IDictionary<string, object>) result.Single();
            CollectionAssert.AreEquivalent(new [] { "ServerId", "ServerName", "DateTime"}, kv.Keys);
        }

        [Test]
        public void DataContainsMultipleRecords()
        {
            var records = new List<ResultRecord>
            {
                new ResultRecord
                {
                    ServerId = "a",
                    SessionRowId = 1,
                    DataJson = "[{\"DateTime\":\"2020-05-03T13:38:28.467\"}, {\"DateTime\":\"2020-05-04T13:38:28.467\"}]"
                }
            };

            var svc = TestBootstrapper.Scope.Resolve<Tabularizer>();
            var result = svc.Process(records);
            Console.WriteLine(JsonConvert.SerializeObject(result));
            var kv = (IDictionary<string, object>)result.Single();
            CollectionAssert.AreEquivalent(new[] { "ServerId", "ServerName", "Rows" }, kv.Keys);
        }
    }
}
