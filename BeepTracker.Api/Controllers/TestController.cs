using Microsoft.AspNetCore.Mvc;

namespace BeepTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        public readonly IConfiguration _configuration;

        public TestController(ILogger<TestController> logger, IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            var connectionString = _configuration.GetConnectionString("BeepTrackerConnection");
            return $"Hello, the site is working? This is the connection string {connectionString}";
        }
    }
}
