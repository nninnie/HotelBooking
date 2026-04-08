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
                    "Skapa ny",
                    "Visa alla",
                    "Visa detaljer",
                    "Uppdatera",
                    "Ta bort",
                    "Återställ",
                    "Tillbaka"
                );

            var choice = AnsiConsole.Prompt(menu);

            return choice switch
            {
                "Skapa ny" => 1,
                "Visa alla" => 2,
                "Visa detaljer" => 3,
                "Uppdatera" => 4,
                "Ta bort" => 5,
                "Återställ" => 6,
                "Tillbaka" => 7,
                _ => 0
            };
        }
    }
}