using BC = BCrypt.Net.BCrypt;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Boiler.Api.Features.Auth.Response;
using Boiler.Api.Features.Auth.Request;
using Boiler.Domain.Auth;
using Boiler.Api.Features.Auth.Helpers;
using Boiler.Infrastructure.Interfaces;
using Boiler.Api.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Api.Features.Auth
{
    internal class AuthService : IAuthService
    {
        private readonly IAuthContext m_Context;
        private readonly AuthSettings m_AuthSettings;
        private readonly IDistributedCache m_DistributedCache;

        public AuthService(IAuthContext context, IOptions<AuthSettings> settings, IDistributedCache distributedCache)
        {
            m_AuthSettings = settings.Value;
            m_Context = context;
            m_DistributedCache = distributedCache;
        }

        public AuthResponse Login(LoginRequest model, string ipAddress)
        {
            var account = m_Context.Accounts.FirstOrDefault(x => x.Email == model.Email);
            if (account == null || (!account.IsVerified && m_AuthSettings.RequiresVerification) || !BC.Verify(model.Password, account.PasswordHash))
                throw new ApiException("Invalid email or password");

            var jwtToken     = GenerateJwtToken(account);
            var refreshToken = GenerateRefreshToken(ipAddress);
            account.RefreshTokens.Add(refreshToken);
            RemoveOldRefreshTokens(account);

            m_Context.Accounts.Update(account);
            m_Context.SaveChanges();

            return new AuthResponse
            {
                Email        = account.Email,
                Role         = account.Role,
                JwtToken     = jwtToken,
                RefreshToken = refreshToken.Token,
            };
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public AuthResponse Register(RegisterRequest model, Role role = Role.User)
        {
            if (m_Context.Accounts.Any(x => x.Email == model.Email))
                throw new ApiException("User is already registered!");

            var account = new Account
            {
                CreationDate      = DateTime.UtcNow,
                Email             = model.Email,
                PasswordHash      = BC.HashPassword(model.Password),
                Role              = role,
                VerificationToken = GenerateTokenString(),
            };

            m_Context.Accounts.Add(account);
            m_Context.SaveChanges();

            // TODO(selim): Send verification mail
            return new AuthResponse
            {
                Email = account.Email,
                Role = account.Role,
                JwtToken = GenerateJwtToken(account),
            };
        }

        public static Account CreateTestAccount(string email, string password, Role role) => new Account
        {
            Email            = email,
            Role             = role,
            CreationDate     = DateTime.Now,
            PasswordHash     = BC.HashPassword(password),
            VerificationDate = DateTime.Now,
        };

        public int GetIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(m_AuthSettings.Secret);
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ClockSkew                = TimeSpan.Zero,
                IssuerSigningKey         = new SymmetricSecurityKey(key),
                ValidateIssuer           = false,
                ValidateAudience         = false,
                ValidateIssuerSigningKey = true,
            }, out _);
            return int.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public async Task<List<AuthResponse>> GetUserListAsync()
        {
            var cacheKey = "UserList";
            List<AuthResponse> userList;
            string serializedUsers;
            var encodedUsers = await m_DistributedCache.GetAsync(cacheKey);

            if (encodedUsers != null)
            {
                serializedUsers = Encoding.UTF8.GetString(encodedUsers);
                userList = JsonConvert.DeserializeObject<List<AuthResponse>>(serializedUsers);
            }
            else
            {
                userList = await m_Context.Accounts.AsNoTracking()
                    .Select(a => new AuthResponse 
                    { 
                        Email = a.Email,
                        Role = a.Role,
                        RefreshToken = a.ResetToken 
                    })
                    .ToListAsync();
                serializedUsers = JsonConvert.SerializeObject(userList);
                encodedUsers = Encoding.UTF8.GetBytes(serializedUsers);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(DateTime.Now.AddHours(3));
                await m_DistributedCache.SetAsync(cacheKey, encodedUsers, options);
            }
            return userList;
            // Ref: https://medium.com/net-core/in-memory-distributed-redis-caching-in-asp-net-core-62fb33925818
            //      https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-5.0
        }

        private static string GenerateTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private string GenerateJwtToken(Account account)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            };

            var key             = Encoding.ASCII.GetBytes(m_AuthSettings.Secret);
            var tokenHandler    = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = m_AuthSettings.AccessTokenTTL == 0 ? DateTime.MaxValue : DateTime.UtcNow.AddMinutes(m_AuthSettings.AccessTokenTTL),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private int? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(m_AuthSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                return accountId;
            }
            catch
            {
                return null;
            }
        }

        private RefreshToken GenerateRefreshToken(string ipAddress) => new()
        {
            Token          = GenerateTokenString(),
            ExpirationDate = DateTime.UtcNow.AddDays(m_AuthSettings.RefreshTokenTTL),
            CreationDate   = DateTime.UtcNow,
            CreatedByIp    = ipAddress
        };

        private void RemoveOldRefreshTokens(Account account) 
        {
            account.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.CreationDate.AddDays(m_AuthSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }
    }
}
