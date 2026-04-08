using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.UI
{
    public static class CrudMenuUI
    {
        public static int Show(string title)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader(title);

            var menu = new SelectionPrompt<string>()
                .Title("[grey]Välj ett alternativ[/]")
                .AddChoices(
                    "Visa alla",
                    "Visa detaljer",
                    "Skapa ny",
                    "Uppdatera",
                    "Ta bort (soft delete)",
                    "Återställ",
                    "Tillbaka"
                );

            // 🔹 ESC-stöd
            AnsiConsole.MarkupLine("[grey]Tryck ESC för att gå tillbaka[/]");
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                return 7; // "Tillbaka"

            var choice = AnsiConsole.Prompt(menu);

            return choice switch
            {
                "Visa alla" => 1,
                "Visa detaljer" => 2,
                "Skapa ny" => 3,
                "Uppdatera" => 4,
                "Ta bort (soft delete)" => 5,
                "Återställ" => 6,
                "Tillbaka" => 7,
                _ => 0
            };
        }
    }
}