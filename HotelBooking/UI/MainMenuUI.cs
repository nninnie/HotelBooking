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

            // Meny
            var menu = new SelectionPrompt<string>()
                .Title("[grey]Välj ett alternativ[/]")
                .HighlightStyle(new Style(foreground: Color.Green))
                .AddChoices(
                    "Hantera bokningar",
                    "Hantera rum",
                    "Hantera gäster",
                    "Hantera betalningar",
                    "Logga ut",
                    "Avsluta"
                );

            var choice = AnsiConsole.Prompt(menu);

            return choice switch
            {
                "Hantera bokningar" => 1,
                "Hantera rum" => 2,
                "Hantera gäster" => 3,
                "Hantera betalningar" => 4,
                "Logga ut" => 5,
                "Avsluta" => 6,
                _ => 0
            };
        }
    }
}