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

                // Användarnamn (kan vara tomt)
                var username = AnsiConsole
                    .Ask<string>("Användarnamn:") ?? "";

                // Lösenord (kan vara tomt)
                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("Lösenord:")
                        .Secret()
                ) ?? "";

                // Allt godkänns
                LoginService.CurrentUserName = username;
                isLoggedIn = true;
                return;
            }
        }
    }
}