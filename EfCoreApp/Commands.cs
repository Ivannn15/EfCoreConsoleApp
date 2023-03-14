using System;
using System.Collections.Generic;
using System.Linq;
using EfCoreApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace EfCoreApp
{ /// <summary>
/// ///////////// добавить изменение для таблицы книги
/// </summary>
    public static class Commands
    {
        // ListAll вывод списка данных в консоль
        public static void ListAll(string item)
        {
            switch (item)
            {
                case "book":
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
                    break;
                case "author":
                    using (var db = new AppDbContext())
                    {
                        foreach (var author in db.Authors.AsNoTracking())
                        {
                            var webUrl = author.WebUrl ?? "- url отсутствует -";
                            Console.WriteLine($"{author.AuthorId} {author.Name} {webUrl}");
                        }
                    }
                    break;
            }
        }

        

        public static void ChangeAuthor() 
        {
            var fieldOfChange = "";
            int IDauthorOfChange = 1;

            using (var db = new AppDbContext())
            {
                do
                {
                    
                    ListAll("author");
                    Console.WriteLine("Выберите id автора для изменения");
                    IDauthorOfChange = Convert.ToInt32(Console.ReadLine());
                    var selectedAuthor = db.Authors.SingleOrDefault(p => p.AuthorId == IDauthorOfChange);
                    if (selectedAuthor == null)
                    {
                        Console.WriteLine("Такого автора нет");
                        continue;
                    }
                    Console.WriteLine($"id - {selectedAuthor.AuthorId}, name - {selectedAuthor.Name}, url - {selectedAuthor.WebUrl}");
                    Console.WriteLine("Выберите поле для изменения \n " +
                        " '1' - name / '2' - url ");
                    
                    fieldOfChange = Console.ReadLine();
                        switch (fieldOfChange)
                        {

                            case "1":
                                Console.WriteLine("Введите значение для поля name");
                                selectedAuthor.Name = Console.ReadLine();
                                db.SaveChanges();
                                Console.WriteLine("Изменения внесены...");
                            Console.WriteLine(" Продолжить изменения - tab any key... / перейти в меню - 'm' \n>");
                            if (Console.ReadLine() == "m")
                            {
                                GetMenu();
                                break;
                            }
                            continue;
                            case "2":
                                Console.WriteLine("Введите значение для поля url");
                                selectedAuthor.WebUrl = Console.ReadLine();
                                db.SaveChanges();
                                Console.WriteLine("Изменения внесены...");
                            Console.WriteLine(" Продолжить изменения - tab any key... / перейти в меню - 'm' ");
                            if (Console.ReadLine() == "m")
                            {
                                GetMenu();
                                break;
                            }
                            continue;
                            default:
                            Console.WriteLine("Вы ввели неверное значение");
                                break;
                        }
                        // добавить изменение для остальных полей как у поля name сделать возвращение в главное меню
                }
                while (true);
            }
        }

        public static void ChangeBook() 
        {
            ListAll("book");

        }

        public static void ChangeAny() 
        {
            Console.WriteLine(" - Для выхода из редактирования нажмите 'q' - ");
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
                case "q":
                    GetMenu();
                    break;
                default:
                    Console.WriteLine("Такого объекта не существует");
                    ChangeAny();
                    break;
            }
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
        /// <returns>возращает false если бд уже создана</returns>
        public static bool WipeCreateSeed(bool onlyIfNoDatabase)
        {
            using (var db = new AppDbContext())
            {
                if (onlyIfNoDatabase && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) 
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

        public static void GetMenu()
        {
            Console.WriteLine(
                "Commands: \nl (список книг), \nc (изменить значение поля для выбранного объекта), \nu (изменить url у последней книги), \nr (пересоздать базу) \ne (выйти) \nдобавьте -l для первых двух команд для просмотра логов");
            Console.Write(
                "Checking if database exists... ");
            Console.WriteLine(Commands.WipeCreateSeed(true) ? "created database and seeded it." : "it exists.");
            Console.Write("> ");
            do
            {
                var command = Console.ReadLine();
                switch (command)
                {
                    case "l":
                        Commands.ListAll("book");
                        break;
                    case "c":
                        Commands.ChangeAny();
                        break;
                    case "l -l":
                        Commands.ListAllWithLogs();
                        break;
                    case "u -l":
                        Commands.ChangeWebUrlWithLogs();
                        break;
                    case "r":
                        Commands.WipeCreateSeed(false);
                        break;
                    case "e":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            } while (true);
        }
    }
}