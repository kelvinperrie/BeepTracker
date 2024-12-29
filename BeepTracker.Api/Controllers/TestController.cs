using Microsoft.AspNetCore.Mvc;

namespace BeepTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "Hello, the site is working.";
        }
    }
}
