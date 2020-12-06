using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Boiler.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Login()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }
    }
}
