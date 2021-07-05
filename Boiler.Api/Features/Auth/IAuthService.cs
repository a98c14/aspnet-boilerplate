using Boiler.Api.Features.Auth.Request;
using Boiler.Api.Features.Auth.Response;
using Boiler.Domain.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Boiler.Api.Features.Auth
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest model, Role role = Role.User);

        AuthResponse Login(LoginRequest model, string ipAddress);

        void Logout();

        int GetIdFromToken(string token);
    }
}