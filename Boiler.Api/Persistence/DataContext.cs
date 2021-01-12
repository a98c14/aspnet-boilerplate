using Boiler.Auth.Entities;
using Boiler.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Boiler.Api.Persistence
{
    public class DataContext : DbContext, IAuthContext
    {
        public DbSet<Account> Accounts { get; set; }

        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("Development"));
        }
    }
}
