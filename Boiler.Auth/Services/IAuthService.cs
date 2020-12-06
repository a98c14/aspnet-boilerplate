using Boiler.Auth.RequestModels;
using Boiler.Auth.ResponseModels;

namespace Boiler.Auth.Services
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest model);
        AuthResponse Login(LoginRequest model, string ipAddress);
        void Logout();
        int GetIdFromToken(string token);
    }
}
