using Spectre.Console;

namespace HotelBooking.UI
{
    public static class CrudMenuUI
    {
        public static string Show(string title, string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader(title, subtitle);

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

            return AnsiConsole.Prompt(menu);
        }
    }
}