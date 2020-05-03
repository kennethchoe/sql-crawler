using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Core;
using SqlCrawler.Backend.Sqlite;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SqlQueriesController : ControllerBase
    {
        private readonly SqlQueryReader _sqlQueryReader;
        private readonly SqlRunner _runner;
        private readonly ResultRepository _resultRepository;
        private readonly Tabularizer _tabularizer;

        public SqlQueriesController(SqlQueryReader sqlQueryReader,
            SqlRunner runner,
            ResultRepository resultRepository,
            Tabularizer tabularizer)
        {
            _sqlQueryReader = sqlQueryReader;
            _runner = runner;
            _resultRepository = resultRepository;
            _tabularizer = tabularizer;
        }

        [HttpGet]
        public IEnumerable<SqlQueryInfo> Get()
        {
            _sqlQueryReader.Reload();
            var sqls = _sqlQueryReader.Read();
            return sqls;
        }

        [HttpGet]
        [Route("{queryName}")]
        public string GetByQuery(string queryName)
        {
            var result = _resultRepository.Get(queryName, null);
            return JsonConvert.SerializeObject(_tabularizer.Process(result));
        }

        [HttpPost]
        [Route("{queryName}/run")]
        public async Task Run(string queryName, CancellationToken token)
        {
            _sqlQueryReader.Reload();
            await _runner.Run(queryName, token);
        }
    }
}
