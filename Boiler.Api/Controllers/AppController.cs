using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
