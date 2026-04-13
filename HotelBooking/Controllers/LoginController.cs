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
                // LOGIN-SKÄRM
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("INLOGGNING", "Ange dina uppgifter");

                AnsiConsole.WriteLine();

                var esc = new TempEsc();

                string? username = esc.AskWithEsc("Användarnamn:");
                if (username == null)
                    return;

                string? password = esc.AskPasswordOrEsc("Lösenord:");
                if (password == null)
                    return;

                var name = username.ToLower();

                // RICHARD
                if (name == "richard")
                {
                    if (password != "bajskorv")
                    {
                        AnsiConsole.MarkupLine("[red]Fel användarnamn eller lösenord![/]");
                        AnsiConsole.MarkupLine("[yellow]Det rätta lösenordet är: [bold]bajskorv[/][/]");
                        AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                        Console.ReadKey();
                        continue;
                    }

                    LoginService.CurrentUserName = "Dungeon Master Richard Chalk";
                    isLoggedIn = true;

                    ShowWelcomeScreen("Richard Chalk", "Dungeon Master");
                    return;
                }

                // NINNIE
                if (name == "ninnie")
                {
                    LoginService.CurrentUserName = $"Manager {username}";
                    isLoggedIn = true;

                    ShowWelcomeScreen(username, "Manager");
                    return;
                }

                // MY & ZOE
                if (name == "my" || name == "zoe")
                {
                    LoginService.CurrentUserName = $"Receptionist {username}";
                    isLoggedIn = true;

                    ShowWelcomeScreen(username, "Receptionist");
                    return;
                }

                // DEFAULT (Guest)
                LoginService.CurrentUserName = $"Guest {username}";
                isLoggedIn = true;

                ShowWelcomeScreen(username, "Guest");
                return;
            }
        }

        // GEMENSAM VÄLKOMSTSKÄRM
        private static void ShowWelcomeScreen(string username, string role)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            AnsiConsole.WriteLine();

            CenteredMarkup($"[green]Välkommen tillbaka, [bold]{username}[/]![/]");
            CenteredMarkup($"[yellow]Roll: [bold]{role}[/][/]");

            AnsiConsole.WriteLine();

            CenteredMarkup("[grey]Tryck på valfri tangent för att fortsätta...[/]");

            Console.ReadKey();
        }

        private static void CenteredMarkup(string markup)
        {
            int width = 70;
            int textLength = markup
                .Replace("[green]", "")
                .Replace("[yellow]", "")
                .Replace("[grey]", "")
                .Replace("[bold]", "")
                .Replace("[/]", "")
                .Length;

            int padding = (width - textLength) / 2;
            if (padding < 0) padding = 0;

            AnsiConsole.MarkupLine(new string(' ', padding) + markup);
        }

        private class TempEsc : EscUI { }
    }
}