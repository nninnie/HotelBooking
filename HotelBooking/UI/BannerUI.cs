using System;

namespace HotelBooking.UI
{
    public static class BannerUI
    {
        private const int BannerWidth = 70;

        public static void ShowLogo()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;

            string[] logoLines =
            {
"▀██▀  ▀██▀           ▄           ▀██  ▀██     ▀██    ██▀ █▀▀▀▀▀██  ",
 "██    ██    ▄▄▄   ▄██▄    ▄▄▄▄   ██   ██      ███  ███      ▄█▀   ",
 "██▀▀▀▀██  ▄█  ▀█▄  ██   ▄█▄▄▄██  ██   ██      █▀█▄▄▀██     ██     ",
 "██    ██  ██   ██  ██   ██       ██   ██      █ ▀█▀ ██   ▄█▀      ",
"▄██▄  ▄██▄  ▀█▄▄█▀  ▀█▄▀  ▀█▄▄▄▀ ▄██▄ ▄██▄    ▄█▄ █ ▄██▄ ██▄▄▄▄▄▄█ ",
"                                                                   "
                                                                   

};

            foreach (var line in logoLines)
                PrintCentered(line);

            Console.ResetColor();
        }

        public static void PrintHeader(string title)
        {
            Console.WriteLine();
            PrintCentered(title.ToUpper());
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void PrintCentered(string text)
        {
            int padding = (BannerWidth - text.Length) / 2;
            if (padding < 0) padding = 0;

            Console.WriteLine(new string(' ', padding) + text);
        }
    }
}