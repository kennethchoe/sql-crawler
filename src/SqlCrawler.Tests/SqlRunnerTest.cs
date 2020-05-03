﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using NUnit.Framework;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Sqlite;

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

            var queryName = "Server DateTime";
            await runner.Run(queryName, new CancellationToken());
            var result = repo.Get(queryName, null);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}