using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SqlCrawler.Backend.Core;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly IAppConfig _config;

        public SettingsController(IAppConfig config)
        {
            _config = config;
        }

        [HttpGet]
        public IEnumerable<KeyValuePair<string, string>> Get()
        {
            return new Dictionary<string, string>
            {
                { "SqlSourceGitRepoUrl", string.Format("<a href='{0}'>{0}</a>", _config.SqlSourceGitRepoUrl) },
                { "CommandTimeoutInSeconds", _config.CommandTimeoutInSeconds.ToString() },
                { "ConnectionTimeoutInSeconds", _config.ConnectionTimeoutInSeconds.ToString() },
            }.ToList();
        }
    }
}
