using FirmManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FirmManager.Data
{
    public class FirmContext : IdentityDbContext<User>
    {
        public FirmContext(DbContextOptions<FirmContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FirmContext>
    {
        public FirmContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<FirmContext>();
            var connectionString = configuration.GetConnectionString("FirmConnection");
            builder.UseSqlServer(connectionString);
            return new FirmContext(builder.Options);
        }
    }
}