using System;
using Microsoft.EntityFrameworkCore;

namespace EfCoreApp
{
    public class AppDbContext : DbContext
    {
        private const string ConnectionString = "server=localhost;user=root;password=root;database=EfCoreTestDb;";
            
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 11)));
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
