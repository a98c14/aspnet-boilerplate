using Boiler.Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Boiler.Infrastructure.Interfaces;
using Boiler.Domain;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace Boiler.Infrastructure
{
    public class DataContext : DbContext, IAuthContext
    {
        public DbSet<Account> Accounts { get; set; }

        private readonly IConfiguration Configuration;
        private readonly IHostEnvironment Env;

        public DataContext(IConfiguration configuration, IHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString(Env.IsDevelopment() ? "Development" : "Production"));
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
               .Entries()
               .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.Now;
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
               .Entries()
               .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}