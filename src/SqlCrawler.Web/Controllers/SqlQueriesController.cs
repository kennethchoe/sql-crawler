using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public SqlQueriesController(SqlQueryReader sqlQueryReader,
            SqlRunner runner,
            ResultRepository resultRepository)
        {
            _sqlQueryReader = sqlQueryReader;
            _runner = runner;
            _resultRepository = resultRepository;
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
        public IEnumerable<dynamic> GetByQuery(string queryName)
        {
            var result = _resultRepository.Get(queryName, null);
            return result;
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
