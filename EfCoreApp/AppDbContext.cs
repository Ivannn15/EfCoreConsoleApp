using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EfCoreApp
{
    public class AppDbContext : DbContext
    {
        // Строка подключения
        private const string ConnectionString = "server=localhost;user=root;password=root;database=EfCoreDb;";
            

        // настройка параметров бд для подключения MySql
        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 11)));
        }

        // Список сущностей таблицы книги включающий авторов
        public DbSet<Book> Books { get; set; }

        ////// Настройка отображения в базу данных
    }
}
