using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System.IO;

namespace EfCoreApp
{

    public static class Commands
    {

        // ListAll вывод списка данных в консоль
        public static void ListAll()
        {
            using (var db = new AppDbContext())
            {
                foreach (var employee in db.Employees.AsNoTracking())
                {
                    Console.WriteLine($"id-{employee.Id} FirstName - {employee.FirstName} LastName - {employee.LastName}");
                    Console.WriteLine("     SalaryPerHour " +
                                      $"{employee.SalaryPerHour}");
                }
            }
        }

        // Изменение сотрудника
        public static void ChangeEmployee() 
        {
            var fieldOfChange = "";
            int IdEmployeeOfchange = 1;

            using (var db = new AppDbContext())
            {
                do
                {
                    
                    ListAll();
                    Console.WriteLine("Выберите id автора для изменения");
                    IdEmployeeOfchange = Convert.ToInt32(Console.ReadLine());
                    var selectedEmployee = db.Employees.SingleOrDefault(p => p.Id == IdEmployeeOfchange);
                    if (selectedEmployee == null)
                    {
                        Console.WriteLine("Такого работника нет");
                        continue;
                    }
                    Console.WriteLine($"id - {selectedEmployee.Id}, FirstName - {selectedEmployee.FirstName}, LastName - {selectedEmployee.LastName}, SalaryPerHour - {selectedEmployee.SalaryPerHour}");
                    Console.WriteLine("Выберите поле для изменения \n " +
                        " '1' - FirstName / '2' - LastName / '3' - SalaryPerHour ");
                    
                    fieldOfChange = Console.ReadLine();
                        switch (fieldOfChange)
                        {

                            case "1":
                                Console.WriteLine("Введите значение для поля FirstName");
                                selectedEmployee.FirstName = Console.ReadLine();
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
                                Console.WriteLine("Введите значение для поля LastName");
                                selectedEmployee.LastName = Console.ReadLine();
                                db.SaveChanges();
                                Console.WriteLine("Изменения внесены...");
                            Console.WriteLine(" Продолжить изменения - tab any key... / перейти в меню - 'm' ");
                            if (Console.ReadLine() == "m")
                            {
                                GetMenu();
                                break;
                            }
                            continue;

                            case "3":
                            Console.WriteLine("Введите значение для поля SalaryPerHour");
                            //var salary = Convert.ToDecimal(Console.ReadLine());
                            if (decimal.TryParse(Console.ReadLine(), out decimal salary))
                            {
                                selectedEmployee.SalaryPerHour = salary;
                                db.SaveChanges();
                                Console.WriteLine("Изменения внесены...");
                                Console.WriteLine(" Продолжить изменения - tab any key... / перейти в меню - 'm' ");
                            }
                            else
                            {
                                Console.WriteLine("> Вы ввели число неверного формата!!!!!\n>Попробуйте еще раз\n");
                                continue;
                            }
                            
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

        // Проверка наличия созданной бд
        public static bool WipeCreateSeed(bool onlyIfNoDatabase)
        {
            using (var db = new AppDbContext())
            {
                if (onlyIfNoDatabase && (db.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) 
                    return false;

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                WriteTestData(db);
                Console.WriteLine("Seeded database");
            }

            return true;
        }

        // Добавление сотрудника
        public static void addEmployee()
        {
            int MaxId;
            using(var db = new AppDbContext())
            {
                MaxId = db.Employees.Max(p => p.Id) + 1;

            Employee employee = new Employee()
            {
                Id = MaxId,
                LastName = "John",
                FirstName = "Doe",
                SalaryPerHour = (decimal)100.50,
            };

                db.Employees.Add(employee);
                db.SaveChanges();
            }
            Console.WriteLine("> Добавлен новый сотрудник");
        }

        // Сохранение изменение и запись их в employees.json
        public static void SaveChanges()
        {
            using (var db = new AppDbContext())
            {
                string json = JsonConvert.SerializeObject(db.Employees, Formatting.Indented);
                File.WriteAllText(@"C:\Users\ivano\source\repos\EfCoreApp\EfCoreApp\bin\Debug\net5.0\employees.json", json);
            }
            Console.WriteLine("Изменения сохранены");
        }

        // Поиск сотрудника по id
        public static void GetEmployee()
        {
            Console.WriteLine("> Введите id сотрудника");
            int idEmp = Convert.ToInt32(Console.ReadLine());

            using (var db = new AppDbContext())
            {
                var tempEmp = db.Employees.SingleOrDefault(p => p.Id == idEmp);

                if (tempEmp == null)
                {
                    Console.WriteLine("> Такого сотрудника не существует");
                }
                else
                {
                    Console.WriteLine($"id-{tempEmp.Id} FirstName - {tempEmp.FirstName} LastName - {tempEmp.LastName}");
                    Console.WriteLine("     SalaryPerHour " +
                                      $"{tempEmp.SalaryPerHour}");
                }
            }
        }

        // Удаление сотрудника
        public static void DeleteEmployee()
        {
            Console.WriteLine("Введите id сотрудника для удаления");
            int idEmp = Convert.ToInt32(Console.ReadLine());

            using (var db = new AppDbContext())
            {
                var tempEmp = db.Employees.SingleOrDefault(p => p.Id == idEmp);

                if (tempEmp == null)
                {
                    Console.WriteLine("> Такого сотрудника не существует");
                }
                else
                {
                    db.Employees.Remove(tempEmp);
                    db.SaveChanges();
                }
            }
            Console.WriteLine("Сотрудник удален");
        }

        // Заполнение бд данными
        public static void WriteTestData(this AppDbContext db)
        {
            string json = File.ReadAllText(@"C:\Users\ivano\source\repos\EfCoreApp\EfCoreApp\bin\Debug\net5.0\employees.json");
            List<Employee> employees = JsonConvert.DeserializeObject<List<Employee>>(json);

            db.Employees.AddRange(employees);
            db.SaveChanges();
        }

        // Список команд для выполения
        public static void GetMenu()
        {
            Console.WriteLine(
                "Commands: \nl (список сотрудников), \nc (изменить значение поля для выбранного объекта), \nd (удалить сотрудника), \na (добавить сотрудника), \ng (поиск сотдуника по id),\ns (сохранить изменения), \ne (выйти)");
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
                        Commands.ListAll();
                        break;
                    case "c":
                        Commands.ChangeEmployee();
                        break;
                    case "e":
                        return;
                    case "g":
                        Commands.GetEmployee();
                        break;
                    case "d":
                        Commands.DeleteEmployee();
                        break;
                    case "s":
                        Commands.SaveChanges();
                        break;
                    case "a":
                        Commands.addEmployee();
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            } while (true);
        }
    }
}