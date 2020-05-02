using Microsoft.AspNetCore.Mvc;

namespace SqlCrawler.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoringController : ControllerBase
    {
        [HttpGet]
        [Route("liveness")]
        public bool Liveness()
        {
            return true;
        }
    }
}
