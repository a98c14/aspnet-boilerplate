using Boiler.Auth.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boiler.Auth.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate m_Next;
        private readonly IConfiguration m_Configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            m_Next = next;
            m_Configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext, IAuthContext dataContext)
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
                await AttachAccountToContext(httpContext, dataContext, token);
            await m_Next(httpContext);

        }

        private async Task AttachAccountToContext(HttpContext context, IAuthContext dataContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(m_Configuration["Auth:Secret"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                context.Items["Account"] = await dataContext.Accounts.FindAsync(accountId);
            }
            catch 
            {
            }
        }
    }
}
