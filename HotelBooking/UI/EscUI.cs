using Spectre.Console;

namespace HotelBooking.UI
{
    public abstract class EscUI
    {
        protected void WaitForEsc()
        {
            AnsiConsole.MarkupLine("[grey]Tryck ESC för att gå tillbaka[/]");

            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                    return;
            }
        }
    }
}