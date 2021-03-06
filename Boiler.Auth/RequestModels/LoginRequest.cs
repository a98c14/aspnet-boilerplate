using System.ComponentModel.DataAnnotations;

namespace Boiler.Auth.RequestModels
{
    public class LoginRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
