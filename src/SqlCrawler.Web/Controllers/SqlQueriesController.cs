using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Core;
using SqlCrawler.Backend.Persistence;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SqlQueriesController : ControllerBase
    {
        private readonly SqlQueryReader _sqlQueryReader;
        private readonly SqlRunner _runner;
        private readonly ResultRepository _resultRepository;
        private readonly SessionRepository _sessionRepository;
        private readonly Tabularizer _tabularizer;

        public SqlQueriesController(SqlQueryReader sqlQueryReader,
            SqlRunner runner,
            ResultRepository resultRepository,
            SessionRepository sessionRepository,
            Tabularizer tabularizer)
        {
            _sqlQueryReader = sqlQueryReader;
            _runner = runner;
            _resultRepository = resultRepository;
            _sessionRepository = sessionRepository;
            _tabularizer = tabularizer;
        }

        [HttpGet]
        public IEnumerable<SqlQueryInfo> Get()
        {
            _sqlQueryReader.Reload();
            var sqls = _sqlQueryReader.Read();
            var sessions = _sessionRepository.Get().ToList();
            sessions.ForEach(s =>
            {
                var sql = sqls.SingleOrDefault(q => q.Name == s.QueryName);
                if (sql != null)
                {
                    sql.LastRetrievedAtUtc = s.FinishedAtUtc;
                }
            });

            return sqls;
        }

        [HttpGet]
        [Route("{queryName}")]
        public SqlQueryInfo GetByQuery(string queryName)
        {
            var sql = _sqlQueryReader.Read().SingleOrDefault(x => x.Name == queryName);
            var session = _sessionRepository.GetByQueryName(queryName);

            if (sql != null && session != null)
                sql.LastRetrievedAtUtc = session.FinishedAtUtc;

            return sql;
        }

        [HttpGet]
        [Route("{queryName}/result")]
        public string GetResult(string queryName)
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

            var sw = new StreamWriter(Response.Body);

            _sqlQueryReader.Reload();
            await _runner.Run(queryName, token, 
                cnt =>
                {
                    Response.ContentLength = cnt;
                },
                async () =>
                {
                    await sw.WriteAsync("1");
                    await sw.FlushAsync();
                }
            );

            return Ok();
        }
    }
}
