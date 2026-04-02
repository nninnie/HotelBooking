using HotelBooking.Core.Service;
using HotelBooking.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{

    public static class LoginController
    {
        public static void Login(ref bool isLoggedIn)
        {
            while (true)
            {
                Console.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("LOGIN");

                Console.Write("Anställningsnummer: ");
                string employeeId = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    ErrorService.Warning("Fältet får inte vara tomt..");
                    continue;
                }

                Console.Write("Lösenord: ");
                string code = Console.ReadLine() ?? "";

                if (string.Equals(code, "bajskorv", StringComparison.OrdinalIgnoreCase) ||
                    string.IsNullOrWhiteSpace(code))
                {
                    LoginService.CurrentUserName = employeeId;
                    isLoggedIn = true;
                    return;
                }

                ErrorService.Show ("Tyvärr, det är såklart 'bajskorv'!");
            }
        }
    }
}
