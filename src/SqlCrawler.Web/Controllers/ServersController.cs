using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using SqlCrawler.Backend;
using SqlCrawler.Backend.Core;
using SqlCrawler.Backend.Persistence;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly SqlCredentialReader _sqlCredentialReader;
        private readonly ResultRepository _resultRepository;

        public ServersController(SqlCredentialReader sqlCredentialReader, 
            ResultRepository resultRepository)
        {
            _sqlCredentialReader = sqlCredentialReader;
            _resultRepository = resultRepository;
        }

        [HttpGet]
        public IEnumerable<SqlServerInfoPublic> Get()
        {
            var servers = _sqlCredentialReader.Read();
            return servers.Select(x => (SqlServerInfoPublic) new SqlServerInfoPublic().InjectFrom(x));
        }

        [HttpGet]
        [Route("{serverId}/result")]
        public string GetResult(string serverId)
        {
            var records = _resultRepository.Get(null, serverId);
            
            return JsonConvert.SerializeObject(records.Select(x =>
                new
                {
                    Rows = x.Data,
                    x.QueryName
                }));
        }
    }
}
