using Spectre.Console;

namespace HotelBooking.UI
{
    public static class MainMenuUI
    {
        public static int ShowMainMenu(bool isLoggedIn)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();

            var title = isLoggedIn ? "Inloggad" : "Huvudmeny";
            BannerUI.PrintHeader(title);

            var menu = new SelectionPrompt<string>()
                .Title("[grey]Använd piltangenterna för att välja och tryck Enter för att fortsätta.[/]")
                .HighlightStyle(new Style(foreground: Color.Green))
                .AddChoices(isLoggedIn
                    ? new[]
                    {
                        "Ny bokning",
                        "Hantera rum",
                        "Hantera gäster",
                        "Hantera bokningar",
                        "Logga ut"
                    }
                    : new[]
                    {
                        "Logga in",
                        "Visa rum",
                        "Visa gäster",
                        "Visa bokningar",
                        "Avsluta"
                    });

            var choice = AnsiConsole.Prompt(menu);

            return choice switch
            {
                "Logga in" => 1,
                "Visa rum" => 2,
                "Visa gäster" => 3,
                "Visa bokningar" => 4,
                "Avsluta" => 5,

                "Ny bokning" => 1,
                "Hantera rum" => 2,
                "Hantera gäster" => 3,
                "Hantera bokningar" => 4,
                "Logga ut" => 5,

                _ => 0
            };
        }
    }
}