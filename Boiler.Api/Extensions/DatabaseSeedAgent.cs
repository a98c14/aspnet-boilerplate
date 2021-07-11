using Boiler.Api.Features.Auth;
using Boiler.Api.Features.Auth.Request;
using Boiler.Domain.Auth;
using Boiler.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boiler.Api.Extensions
{
    public class DatabaseSeedAgent
    {
        private readonly IAuthService m_AuthService;
        private readonly DataContext m_db;

        public DatabaseSeedAgent(IAuthService authService, DataContext db)
        {
            m_AuthService = authService;
            m_db = db;
        }

        public void SeedDatabase()
        {
            if (!m_db.Accounts.Any(a => a.Role == Role.SuperAdmin))
            {
                var superAdminResponse = m_AuthService.Register(
                    new RegisterRequest
                    {
                        Email = "super@admin.com",
                        Password = "123456"
                    },
                    Role.SuperAdmin);
                var adminResponse = m_AuthService.Register(
                    new RegisterRequest
                    {
                        Email = "admin@ecma.com",
                        Password = "123456",
                    },
                    Role.Admin
                    );
            }
        }
    }
}