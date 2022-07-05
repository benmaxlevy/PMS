using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using MySql.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Json;

using PMS.Models;

namespace PMS.Contexts
{
    internal class UserContext: DbContext
    {
        public DbSet<User> User { get; set; } = null!;
        public DbSet<DayIncome> DayIncome { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true,true)
                .Build();

            if (!string.IsNullOrEmpty(configuration.GetConnectionString("Connection")))
            {
                try
                {
                    optionsBuilder.UseMySQL(configuration.GetConnectionString("Connection")!);
                }
                catch (Exception e)
                {
                    throw new Exception(e.ToString());
                }
            }
            else
            {
                throw new Exception("Connection string not found");
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.HashedPassword).IsRequired();
                entity.Property(e => e.HourlyRate).IsRequired();
            });
            
            modelBuilder.Entity<DayIncome>(entity =>
            {
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Income).IsRequired();
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Incomes);
            });
        }
    }
}