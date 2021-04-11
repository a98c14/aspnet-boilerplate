using Boiler.Core.Auth.Entities;
using Microsoft.EntityFrameworkCore;
using Boiler.Auth.Interfaces;

namespace Boiler.Core.Tests
{
    internal class MockContext : DbContext, IAuthContext
    {
        public DbSet<Account> Accounts { get; set; }

        public MockContext(DbContextOptions<MockContext> options) : base(options) { }
    }
}
