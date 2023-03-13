﻿using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace EfCoreApp
{
    public static class Commands
    {
        // ListAll вывод списка данных в консоль
        public static void ListAll()
        {
            using (var db = new AppDbContext()) // Создание объекта класса Контекст для получения данных из бд
            {
                foreach (var book in db.Books.AsNoTracking() // Считывание всех книг из базы без возможности изменений
                    .Include(book => book.Author)) // добавление в запрос информации об авторе
                {
                    var webUrl = book.Author.WebUrl ?? "- url отсутствует -";
                    Console.WriteLine($"{book.Title} by {book.Author.Name}");
                    Console.WriteLine("     Опубликованно " +
                                      $"{book.PublishedOn:dd-MMM-yyyy}. {webUrl}");
                }
            }
        }

        public static void ChangeAuthor() 
        {
            
            int fieldOfChange = 0;
            int authorOfChange;

            using (var db = new AppDbContext())
            {
                foreach (var author in db.Books.Include(book => book.Author)) // разделить вывод авторов и внесение изменений
                {

                    Console.WriteLine($" {author.AuthorId} {author.Author.Name} {author.Author.WebUrl} ");
                    Console.WriteLine("Выберите id автора для изменения");
                    authorOfChange = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine(" Выберите поле для изменения \t " +
                        " '1' - name / '2' - url ");
                    fieldOfChange = Convert.ToInt32(Console.ReadLine());

                    switch (fieldOfChange)
                    {

                        case 1:
                            Console.WriteLine("Введите значение для поля name");
                            author.Author.Name = Console.ReadLine();
                            break;
                        default:
                            break;
                    }

                    Console.WriteLine(" Для сохранения нажмине S ");
                }

                
            }
        }

        public static void ChangeBook() 
        {

        }

        public static void ChangeAny() 
        {
            Console.WriteLine("Выберите тип объекта изменения указав номер из списка");
            Console.WriteLine("'1' - Автор / '2' - Книга");
            var typeOfChange = Console.ReadLine();
            switch (typeOfChange)
            {
                case "1":
                    ChangeAuthor();
                    break;
                case "2":
                    ChangeBook();
                    break;
                default: Console.WriteLine("adsf"); break;
            }
        }

        // CgangeWebUrl изменение url
        public static void ChangeWebUrl()
        {
            Console.WriteLine("Введите url для книги Quantum Networking > ");
            var newWebUrl = Console.ReadLine(); // Считываем новый url

            using (var db = new AppDbContext())
            {
                var singlebook = db.Books
                    .Include(book => book.Author)
                    .Single(book => book.Title == "Quantum Networking"); // выбор единственного экземпляра по названию
                singlebook.Author.WebUrl = newWebUrl;
                db.SaveChanges(); // внесение изменений в бд
                Console.WriteLine("... Данные сохранены.");
            }
            ListAll();

        }

        public static void ListAllWithLogs()
        {
            var logs = new List<string>();
            using (var db = new AppDbContext())
            {
                var serviceProvider = db.GetInfrastructure();
                var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

                foreach (var entity in
                    db.Books.AsNoTracking()
                        .Include(book => book.Author))
                {
                    var webUrl = entity.Author.WebUrl == null
                        ? "- no web url given -"
                        : entity.Author.WebUrl;
                    Console.WriteLine(
                        $"{entity.Title} by {entity.Author.Name}");
                    Console.WriteLine("     " +
                                      $"Published on {entity.PublishedOn:dd-MMM-yyyy}" +
                                      $". {webUrl}");
                }
            }

            Console.WriteLine("---------- LOGS ------------------");
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }

        public static void ChangeWebUrlWithLogs()
        {
            var logs = new List<string>();
            Console.Write("New Quantum Networking WebUrl > ");
            var newWebUrl = Console.ReadLine();

            using (var db = new AppDbContext())
            {
                var serviceProvider = db.GetInfrastructure();
                var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

                var singleBook = db.Books
                    .Include(book => book.Author)
                    .Single(b => b.Title == "Quantum Networking");
                singleBook.Author.WebUrl = newWebUrl;
                db.SaveChanges();
                Console.Write("... SavedChanges called.");
            }

            Console.WriteLine("---------- LOGS ------------------");
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }

        /// <summary>
        ///     This will wipe and create a new database - which takes some time
        /// </summary>
        /// <param name="onlyIfNoDatabase">если = true то бд не создается</param>
        /// <returns>returns true if database database was created</returns>
        public static bool WipeCreateSeed(bool onlyIfNoDatabase)
        {
            using (var db = new AppDbContext())
            {
                if (onlyIfNoDatabase && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) // возвращает false если база данных уже существует
                    return false;

                db.Database.EnsureDeleted(); // перезапись базы данных
                db.Database.EnsureCreated(); // перезапись базы данных

                WriteTestData(db);
                Console.WriteLine("Seeded database");
                
            }

            return true;
        }

        // Заполнение бд тестовыми данными
        public static void WriteTestData(this AppDbContext db)
        {
            Author martinFowler = new Author
            {
                Name = "Martin Fowler",
                WebUrl = "http://martinfowler.com/"
            };

            Author ericEvans = new Author
            {
                Name = "Eric Evans",
                WebUrl = "http://domainlanguage.com/"
            };

            Author futurePerson = new Author
            {
                Name = "Future Person"
            };

            List<Book> books = new List<Book> // список книг
            {
                new Book
                {
                    Title = "Refactoring",
                    Description = "Improving the design of existing code",
                    PublishedOn = new DateTime(1999, 7, 8),
                    Author = martinFowler
                },
                new Book
                {
                    Title = "Patterns of Enterprise Application Architecture",
                    Description = "Written in direct response to the stiff challenges",
                    PublishedOn = new DateTime(2002, 11, 15),
                    Author = martinFowler
                },
                new Book
                {
                    Title = "Domain-Driven Design",
                    Description = "Linking business needs to software design",
                    PublishedOn = new DateTime(2003, 8, 30),
                    Author = ericEvans
                },
                new Book
                {
                    Title = "Quantum Networking",
                    Description = "Entangled quantum networking provides faster-than-light data communications",
                    PublishedOn = new DateTime(2057, 1, 1),
                    Author = futurePerson
                }
            };

            db.Books.AddRange(books);
            db.SaveChanges();
        }
    }
}