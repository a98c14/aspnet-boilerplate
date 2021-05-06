using System.ComponentModel.DataAnnotations;

namespace Boiler.Api.Features.Auth.Request
{
    public class LoginRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
