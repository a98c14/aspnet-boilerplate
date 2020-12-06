using Boiler.Auth.RequestModels;
using Boiler.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Boiler.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService m_AuthService;

        public AuthController(IAuthService authService)
        {
            m_AuthService = authService;
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = m_AuthService.Register(model);
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = m_AuthService.Login(model, HttpContext.Connection.RemoteIpAddress.ToString());
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            return Ok();
        }
    }
}
