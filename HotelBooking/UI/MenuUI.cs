using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.UI
{
    public static class MenuUI
    {
        private const string Highlight = "\u001b[32m";

        public static int ShowMainMenu(bool isLoggedIn)
        {
            int selected = 1;
            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();
                BannerUI.ShowLogo();

                if (!isLoggedIn) 
                {
                    BannerUI.PrintHeader("Main Menu");

                    PrintOption(1, selected, "Log in");
                    PrintOption(2, selected, "View rooms");
                    PrintOption(3, selected, "View guests");
                    PrintOption(4, selected, "View bookings");
                    PrintOption(5, selected, "Exit");
                }
                else
                {
                    BannerUI.PrintHeader("Logged in");

                    PrintOption(1, selected, "New booking");
                    PrintOption(2, selected, "Manage rooms");
                    PrintOption(3, selected, "Manage guests");
                    PrintOption(4, selected, "Manage bookings");
                    PrintOption(5, selected, "Log out");
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.DownArrow) selected++;
                if (key.Key == ConsoleKey.UpArrow) selected--;

                int max = 5;
                if (selected < 1) selected = max;
                if (selected > max) selected = 1;

                if (key.Key == ConsoleKey.Enter)
                {
                    while (Console.KeyAvailable)
                        Console.ReadKey(true);

                    return selected;
                }
            }
        }

        private static void PrintOption(int index, int selected, string text)
        {
            if (index == selected)
                Console.WriteLine($"·{Highlight}{text}\u001b[0m");
            else
                Console.WriteLine($"  {text}");
        }
    }
}
