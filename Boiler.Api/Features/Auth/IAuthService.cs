using Boiler.Api.Features.Auth.Request;
using Boiler.Api.Features.Auth.Response;

namespace Boiler.Api.Features.Auth
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest model);
        AuthResponse Login(LoginRequest model, string ipAddress);
        void Logout();
        int GetIdFromToken(string token);
    }
}
