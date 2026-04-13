using Spectre.Console;

namespace HotelBooking.UI
{
    public static class CrudMenuUI
    {
        public static string? Show(string title, string subtitle, params string[] options)
        {
            int index = 0;

            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader(title, subtitle);

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == index)
                        AnsiConsole.MarkupLine($"[yellow]> {options[i]}[/]");
                    else
                        AnsiConsole.MarkupLine($"  {options[i]}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index = (index - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        index = (index + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        return options[index];

                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }
    }
}