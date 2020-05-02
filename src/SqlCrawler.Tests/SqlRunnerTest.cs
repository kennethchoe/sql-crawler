using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;

namespace SqlCrawler.Tests
{
    [TestFixture]
    class SqlRunnerTest
    {
        [Test]
        public async Task Run()
        {
            var scope = TestBootstrapper.Scope;
            var runner = scope.Resolve<SqlRunner>();
            var result = await runner.Run("Server DateTime", new CancellationToken());
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}
