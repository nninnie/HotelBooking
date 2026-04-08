using Spectre.Console;

namespace HotelBooking.UI
{
    public static class MainMenuUI
    {
        public static int ShowMainMenu(string currentUser)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader($"Huvudmeny - Inloggad som {currentUser}");

            var menu = new SelectionPrompt<string>()
                .Title("[grey]Välj ett alternativ[/]")
                .HighlightStyle(new Style(foreground: Color.Green))
                .AddChoices(
                    "Ny bokning",
                    "Hantera bokningar",
                    "Hantera rum",
                    "Hantera gäster",
                    "Hantera betalningar",
                    "Logga ut",
                    "Avsluta"
                );

            // 🔹 ESC-stöd
            AnsiConsole.MarkupLine("[grey]Tryck ESC för att gå tillbaka[/]");
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
                return 0;

            var choice = AnsiConsole.Prompt(menu);

            return choice switch
            {
                "Ny bokning" => 1,
                "Hantera bokningar" => 2,
                "Hantera rum" => 3,
                "Hantera gäster" => 4,
                "Hantera betalningar" => 5,
                "Logga ut" => 6,
                "Avsluta" => 7,
                _ => 0
            };
        }
    }
}