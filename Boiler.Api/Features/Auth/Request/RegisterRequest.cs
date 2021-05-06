using System.ComponentModel.DataAnnotations;

namespace Boiler.Api.Features.Auth.Request
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
