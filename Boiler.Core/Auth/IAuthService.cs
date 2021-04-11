using Boiler.Core.Auth.Request;
using Boiler.Core.Auth.Response;

namespace Boiler.Core.Auth
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest model);
        AuthResponse Login(LoginRequest model, string ipAddress);
        void Logout();
        int GetIdFromToken(string token);
    }
}
