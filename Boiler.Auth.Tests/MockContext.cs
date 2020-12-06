using Boiler.Auth.Entities;
using Boiler.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Boiler.Auth.Tests
{
    internal class MockContext : DbContext, IAuthContext
    {
        public DbSet<Account> Accounts { get; set; }

        public MockContext(DbContextOptions<MockContext> options) : base(options) { }
    }
}
