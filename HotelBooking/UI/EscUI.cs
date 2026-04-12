using Spectre.Console;

namespace HotelBooking.UI
{
    // Bas-klass för all UI som ska stödja ESC
    public abstract class EscUI
    {
        public string? CurrentHeader { get; set; }

        // Väntar på att användaren trycker ESC för att gå tillbaka
        public void WaitForEsc()
        {
            AnsiConsole.MarkupLine("\n[grey]Tryck ESC för att gå tillbaka[/]");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                    return;
            }
        }

        // Textinput som kan avbrytas med ESC
        public string? AskWithEsc(string question)
        {
            AnsiConsole.Markup($"{question} ");

            string input = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                    return null;

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        // Lista med piltangenter + ESC för att avbryta
        public T? SelectWithEsc<T>(List<T> items, Func<T, string> converter, string? header = null)
        {
            int index = 0;

            while (true)
            {
                AnsiConsole.Clear();

                BannerUI.ShowLogo();

                // ANVÄND HEADER-PARAMETER ELLER CurrentHeader
                string? h = header ?? CurrentHeader;
                if (!string.IsNullOrWhiteSpace(h))
                    BannerUI.PrintHeader(h);

                for (int i = 0; i < items.Count; i++)
                {
                    if (i == index)
                        AnsiConsole.MarkupLine($"[yellow]> {converter(items[i])}[/]");
                    else
                        AnsiConsole.MarkupLine($"  {converter(items[i])}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index = (index - 1 + items.Count) % items.Count;
                        break;

                    case ConsoleKey.DownArrow:
                        index = (index + 1) % items.Count;
                        break;

                    case ConsoleKey.Enter:
                        return items[index];

                    case ConsoleKey.Escape:
                        return default;
                }
            }
        }
    }
}