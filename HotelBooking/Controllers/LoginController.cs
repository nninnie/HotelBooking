using HotelBooking.Core.Service;
using HotelBooking.UI;
using Spectre.Console;

namespace HotelBooking.Controllers
{
    public static class LoginController
    {
        public static void Login(ref bool isLoggedIn)
        {
            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("LOGIN");

                // Frågar efter användarnamn.
                // Tom input → använd "admin" som standard.
                var employeeId = AnsiConsole.Ask<string>("Användarnamn:");
                if (string.IsNullOrWhiteSpace(employeeId))
                    employeeId = "admin";

                // Frågar efter lösenord (dolt).
                // Tom input → använd "admin" som standard.
                var code = AnsiConsole.Prompt(
                    new TextPrompt<string>("Lösenord:")
                        .Secret()
                );
                if (string.IsNullOrWhiteSpace(code))
                    code = "admin";

                // Godkänn admin/admin (eller tom/tom som blir admin/admin).
                if (employeeId == "admin" && code == "admin")
                {
                    LoginService.CurrentUserName = employeeId;
                    isLoggedIn = true;
                    return;
                }

                // Felaktiga uppgifter → visa felmeddelande.
                ErrorService.Show("Fel användarnamn eller lösenord.");
            }
        }
    }
}