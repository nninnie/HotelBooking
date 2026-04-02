using HotelBooking.Core.Service;
using HotelBooking.Data;
using HotelBooking.UI;
using Spectre.Console;

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
                BannerUI.PrintHeader("LOGIN");

                var username = AnsiConsole
                    .Ask<string>("Användarnamn:") ?? "";

                var password = AnsiConsole
                    .Prompt(
                    new TextPrompt<string>("Lösenord:")
                    .Secret()
                ) ?? "";

                // Hash funktion som gör lösenordet “oläsbart”
                string Hash(string input) => input
                    .GetHashCode()
                    .ToString();

                // Försök hitta användaren i databasen
                var user = db.Employees
                    .FirstOrDefault(e => e
                    .Username == username);

                if (user != null && user
                    .PasswordHash == Hash(password))
                {
                    // RIKTIG LOGIN
                    LoginService.CurrentUserName = user.FullName;
                    isLoggedIn = true;
                    return;
                }

                // FEL LOGIN - visa felsökningsmeddelande
                AnsiConsole.MarkupLine("[red]Fel användarnamn eller lösenord![/]");
                AnsiConsole.MarkupLine("[yellow]Det rätta lösenordet är: [bold]bajskorv[/][/]");
                AnsiConsole.MarkupLine("[grey]Tryck på valfri tangent för att fortsätta...[/]");
                Console.ReadKey();

                // DUMMY-LOGIN - för lätt kunna felsöka UI:t utan att behöva logga in på riktigt
                LoginService.CurrentUserName = username;
                isLoggedIn = true;
                return;
            }
        }
    }
}