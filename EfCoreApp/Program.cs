using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;

namespace EfCoreApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "Commands: l (список книг), u (изменить url у последней книги), r (пересоздать базу) and e (выйти) - добавьте -l для первых двух команд для просмотра логов");
            Console.Write(
                "Checking if database exists... ");
            Console.WriteLine(Commands.WipeCreateSeed(false) ? "created database and seeded it." : "it exists.");
            do
            {
                Console.Write("> ");
                var command = Console.ReadLine();
                switch (command)
                {
                    case "l":
                        Commands.ListAll();
                        break;
                    case "u":
                        Commands.ChangeWebUrl();
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
                        Console.WriteLine("Unknown command.");
                        break;
                }
            } while (true);
        }

        
    }
}
