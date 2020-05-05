using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly SqlCredentialReader _sqlCredentialReader;
        private readonly ResultRepository _resultRepository;
        private readonly Tabularizer _tabularizer;

        public SqlQueriesController(SqlQueryReader sqlQueryReader,
            SqlRunner runner,
            SqlCredentialReader sqlCredentialReader,
            ResultRepository resultRepository,
            Tabularizer tabularizer)
        {
            _sqlQueryReader = sqlQueryReader;
            _runner = runner;
            _sqlCredentialReader = sqlCredentialReader;
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
        public async Task<ActionResult> Run(string queryName, CancellationToken token)
        {
            Response.StatusCode = 200;
            Response.ContentType = "text/event-stream";
            var serverCount = _sqlCredentialReader.Read().Count();
            Response.ContentLength = serverCount;

            var sw = new StreamWriter(Response.Body);

            _sqlQueryReader.Reload();
            await _runner.Run(queryName, token, async () =>
            {
                await sw.WriteAsync("1");
                await sw.FlushAsync();
            });
            return null;
        }
    }
}
