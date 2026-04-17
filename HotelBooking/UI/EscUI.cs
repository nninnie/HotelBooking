using HotelBooking.Services.Interfaces;
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
            AnsiConsole.Markup(question);

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
        // Numerisk input som kan avbrytas med ESC
        public int? AskIntOrEsc(string question, int? defaultValue = null)
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

                    // ⭐ ENTER = behåll värde
                    if (string.IsNullOrWhiteSpace(input))
                        return defaultValue;

                    if (int.TryParse(input, out int value))
                        return value;

                    AnsiConsole.MarkupLine("[red]Ogiltigt nummer. Försök igen.[/]");
                    input = "";
                    AnsiConsole.Markup($"{question} ");
                    continue;
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
                BannerUI.PrintHeader("Hantera bokningar", "Skapa ny bokning");

                AnsiConsole.WriteLine();

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

        
        public static int? SelectExistingGuest(EscUI ui, IGuestService guestService, string subtitle)
        {
            var guests = guestService.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]");
                ui.WaitForEsc();
                return null;
            }

            int pageSize = 10;
            int page = 0;
            int index = 0;

            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("Hantera bokningar", subtitle);

                int totalPages = (int)Math.Ceiling(guests.Count() / (double)pageSize);

                var pageItems = guests
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (index >= pageItems.Count)
                    index = pageItems.Count - 1;

                AnsiConsole.MarkupLine($"[grey]Sida {page + 1} / {totalPages}[/]");
                AnsiConsole.WriteLine();

                for (int i = 0; i < pageItems.Count; i++)
                {
                    var g = pageItems[i];
                    string text = $"{g.LastName}, {g.FirstName} – {g.Email} – {g.PhoneNumber}";

                    if (i == index)
                        AnsiConsole.MarkupLine($"[yellow]> {text}[/]");
                    else
                        AnsiConsole.MarkupLine($"  {text}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index--;
                        if (index < 0)
                        {
                            if (page > 0)
                            {
                                page--;
                                index = pageSize - 1;
                            }
                            else index = 0;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        index++;
                        if (index >= pageItems.Count)
                        {
                            if (page < totalPages - 1)
                            {
                                page++;
                                index = 0;
                            }
                            else index = pageItems.Count - 1;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (page > 0)
                        {
                            page--;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (page < totalPages - 1)
                        {
                            page++;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.Enter:
                        return pageItems[index].Id;

                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }
        // Decimalinput som kan avbrytas med ESC
        public decimal? AskDecimalOrEsc(string question)
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


                    if (string.IsNullOrWhiteSpace(input))
                        return null;

                    if (decimal.TryParse(input, out decimal value))
                        return value;

                    AnsiConsole.MarkupLine("[red]Ogiltigt belopp. Försök igen.[/]");
                    input = "";
                    AnsiConsole.Markup($"{question}: ");
                    continue;
                }

                // BACKSPACE
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                // Tillåt endast siffror och komma
                if (char.IsDigit(key.KeyChar) || key.KeyChar == ',' || key.KeyChar == '.')
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        // Lösenordsinput som kan avbrytas med ESC
        public string? AskPasswordOrEsc(string question)
        {
            AnsiConsole.Markup($"{question} ");

            string input = "";
            ConsoleKeyInfo key;

            while (true)
            {
                key = Console.ReadKey(true);

                // ESC → avbryt
                if (key.Key == ConsoleKey.Escape)
                    return null;

                // ENTER → returnera lösenordet
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input;
                }

                // BACKSPACE
                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input[..^1];
                    Console.Write("\b \b");
                    continue;
                }

                // Maskera lösenordet
                if (!char.IsControl(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write("*");
                }
            }
        }

        public bool? AskBoolOrEsc(string question, bool defaultValue)
        {
            AnsiConsole.Markup($"{question}: ");

            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.J: Console.WriteLine(); return true;
                    case ConsoleKey.N: Console.WriteLine(); return false;
                    case ConsoleKey.Enter: Console.WriteLine(); return defaultValue;
                    case ConsoleKey.Escape: Console.WriteLine(); return null;
                }
            }
        }

        public bool? AskBoolOrEsc(bool defaultValue)
        {
            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.J: return true;
                    case ConsoleKey.N: return false;
                    case ConsoleKey.Enter: return defaultValue;
                    case ConsoleKey.Escape: return null;
                }
            }
        }



        public int? AskIntOrEsc(string question, int defaultValue)
        {
            var result = AskIntOrEsc(question);
            return result == null && Console.CursorLeft == 0 ? defaultValue : result;
        }


        public decimal? AskDecimalOrEsc(string question, decimal defaultValue)
        {
            var result = AskDecimalOrEsc(question);
            return result == null && Console.CursorLeft == 0 ? defaultValue : result;
        }

    }
}