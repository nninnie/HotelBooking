using System;
using System.Collections.Generic;
using System.Text;
using HotelBooking.UI;


namespace HotelBooking.Controllers
{
    public static class MainMenuController
    {
        public static void Start()
        {
            bool isLoggedIn = false;

            while (true)
            {
                int menuchoice = MenuUI.ShowMainMenu(isLoggedIn);

                switch (menuchoice)
                {
                    case 1:
                        LoginController.Login(ref isLoggedIn);
                        continue;
                    case 2:
                        // View rooms
                        continue;
                    case 3:
                        // View guests
                        continue;
                    case 4:
                        // View bookings
                        continue;
                    case 5:
                        Console.WriteLine("\n\n\n\t     Tack för idag! Ha en fin dag ❤️\n");
                        Environment.Exit(0);
                        break;
                }
                Console.ReadKey(true);
            }
        }
    }
}
