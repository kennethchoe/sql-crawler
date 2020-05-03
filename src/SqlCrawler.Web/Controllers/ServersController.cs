using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Omu.ValueInjecter;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly SqlCredentialReader _sqlCredentialReader;

        public ServersController(SqlCredentialReader sqlCredentialReader)
        {
            _sqlCredentialReader = sqlCredentialReader;
        }

        [HttpGet]
        public IEnumerable<SqlServerInfoPublic> Get()
        {
            var servers = _sqlCredentialReader.Read();
            return servers.Select(x => (SqlServerInfoPublic) new SqlServerInfoPublic().InjectFrom(x));
        }
    }
}
