using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Persistence;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class SqlRunnerTest
    {
        [Test]
        public async Task Run()
        {
            var scope = TestBootstrapper.Scope;
            var repo = scope.Resolve<ResultRepository>();
            var runner = scope.Resolve<SqlRunner>();

            var queryName = "1. Parameterized Sqls";
            await runner.Run(queryName, new CancellationToken(), _ => { }, () => { });
            var result = repo.Get(queryName, null);
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
