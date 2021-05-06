using Boiler.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Boiler.Infrastructure.Interfaces;

namespace Boiler.Core.Tests
{
    internal class MockContext : DbContext, IAuthContext
    {
        public DbSet<Account> Accounts { get; set; }

        public MockContext(DbContextOptions<MockContext> options) : base(options) { }
    }
}
