using HotelBooking.Data;
using HotelBooking.UI;
using Spectre.Console;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Core;

namespace HotelBooking.Controllers
{
    public static class LoginController
    {
        public static void Login(HotelDBContext db, ref bool isLoggedIn)
        {
            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("INLOGGNING");

                AnsiConsole.MarkupLine("");
                var username = AnsiConsole.Ask<string>("[yellow]Användarnamn:[/]");
                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("[yellow]Lösenord:[/]")
                        .Secret()
                        .PromptStyle("white")
                );

                // Gör användarnamnet case-insensitive
                var name = username.ToLower();

                if (name == "richard")
                {
                    // Fel lösenord - visa felmeddelande men logga ändå in
                    if (password != "bajskorv")
                    {
                        AnsiConsole.MarkupLine("[red]Fel användarnamn eller lösenord![/]");
                        AnsiConsole.MarkupLine("[yellow]Det rätta lösenordet är: [bold]bajskorv[/][/]");
                        AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                        Console.ReadKey();
                    }

                    // Logga in som Dungeon Master
                    LoginService.CurrentUserName = "Dungeon Master Richard Chalk";
                    isLoggedIn = true;

                    AnsiConsole.Clear();
                    BannerUI.ShowLogo();
                    AnsiConsole.MarkupLine("");
                    AnsiConsole.MarkupLine("[green]Välkommen tillbaka, [bold]Richard Chalk[/]![/]");
                    AnsiConsole.MarkupLine("[yellow]Roll: [bold]Dungeon Master[/][/]");
                    AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                    Console.ReadKey();

                    return;
                }

                if (name == "ninnie")
                {
                    LoginService.CurrentUserName = $"Manager {username}";
                    isLoggedIn = true;

                    AnsiConsole.Clear();
                    BannerUI.ShowLogo();
                    AnsiConsole.MarkupLine("");
                    AnsiConsole.MarkupLine($"[green]Välkommen tillbaka, [bold]{username}[/]![/]");
                    AnsiConsole.MarkupLine("[yellow]Roll: [bold]Manager[/][/]");
                    AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                    Console.ReadKey();

                    return;
                }

    
                if (name == "my" || name == "zoe")
                {
                    LoginService.CurrentUserName = $"Receptionist {username}";
                    isLoggedIn = true;

                    AnsiConsole.Clear();
                    BannerUI.ShowLogo();
                    AnsiConsole.MarkupLine("");
                    AnsiConsole.MarkupLine($"[green]Välkommen tillbaka, [bold]{username}[/]![/]");
                    AnsiConsole.MarkupLine("[yellow]Roll: [bold]Receptionist[/][/]");
                    AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                    Console.ReadKey();

                    return;
                }


                LoginService.CurrentUserName = $"Guest {username}";
                isLoggedIn = true;

                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"[green]Välkommen tillbaka, [bold]{username}[/]![/]");
                AnsiConsole.MarkupLine("[yellow]Roll: [bold]Guest[/][/]");
                AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                Console.ReadKey();

                return;
            }
        }
    }
}