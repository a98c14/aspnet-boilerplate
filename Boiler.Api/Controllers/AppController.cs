using Microsoft.AspNetCore.Mvc;

namespace Boiler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppController : ControllerBase
    {
        [HttpGet]
        [Route("Test")]
        public IActionResult Test()
        {
            return Ok("Its ok :)");
        }
    }
}
