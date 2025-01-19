using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeepTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
            var theUser = this.User.Identity.Name;
            return $"Hello {theUser}, the site is working!? ";
        }
    }
}
