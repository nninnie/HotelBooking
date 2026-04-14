using Spectre.Console;

public static class BannerUI
{
    public static void ShowLogo()
    {
        AnsiConsole.WriteLine();
        SpectreCenter.Center("[blue]╦ ╦┌─┐┌┬┐┌─┐┬  ┬    ╔╦╗╔═╗[/]");
        SpectreCenter.Center("[blue]╠═╣│ │ │ ├┤ │  │    ║║║╔═╝[/]");
        SpectreCenter.Center("[blue]╩ ╩└─┘ ┴ └─┘┴─┘┴─┘  ╩ ╩╚═╝[/]");
        AnsiConsole.WriteLine();
    }

    public static void PrintHeader(string title, string? subtitle = null)
    {
        SpectreCenter.Center($"[white]{title.ToUpper()}[/]");

        if (!string.IsNullOrWhiteSpace(subtitle))
            SpectreCenter.Center($"[grey]{Markup.Escape(subtitle)}[/]");

        AnsiConsole.WriteLine();
    }
}