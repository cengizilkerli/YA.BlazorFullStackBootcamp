using Microsoft.EntityFrameworkCore;
using PasswordStorageApp.Domain.Models;

namespace PasswordStorageApp.WebAPI.Persistens.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseInMemoryDatabase("PasswordStorageApp");
            optionsBuilder.UseSqlite("Data Source=PasswordStorageApp.db;");

            base.OnConfiguring(optionsBuilder);
        }

    }
}