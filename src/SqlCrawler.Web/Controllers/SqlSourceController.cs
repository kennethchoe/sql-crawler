using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using SqlCrawler.Backend;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SqlSourceController : ControllerBase
    {
        private readonly SqlSourceReader _sqlSourceReader;

        public SqlSourceController(SqlSourceReader sqlSourceReader)
        {
            _sqlSourceReader = sqlSourceReader;
        }

        [HttpGet]
        public IEnumerable<SqlQueryInfo> Get()
        {
            _sqlSourceReader.Reload();
            var sqls = _sqlSourceReader.Read();
            return sqls;
        }
    }
}
