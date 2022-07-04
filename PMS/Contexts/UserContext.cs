using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore;
using PMS.Models;

namespace PMS.Contexts
{
    internal class UserContext: DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<DayIncome> DayIncome { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=pms;user=root;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Username).IsRequired();
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