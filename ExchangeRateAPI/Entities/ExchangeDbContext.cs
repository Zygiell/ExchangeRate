using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRateAPI.Entities
{
    public class ExchangeDbContext : DbContext
    {
        public ExchangeDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Cache> Caches { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cache>()
                .HasKey(k => new { k.FirstCurrency, k.SecondCurrency, k.Date });
        }
    }
}
