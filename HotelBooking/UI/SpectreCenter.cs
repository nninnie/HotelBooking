using Spectre.Console;

public static class SpectreCenter
{
    private const int Width = 70;

    public static void Center(string markup)
    {
        int textLength = StripMarkup(markup).Length;
        int padding = (Width - textLength) / 2;
        if (padding < 0) padding = 0;

        AnsiConsole.MarkupLine(new string(' ', padding) + markup);
    }

    private static string StripMarkup(string markup)
    {
        bool inside = false;
        var result = new List<char>();

        foreach (char c in markup)
        {
            if (c == '[') { inside = true; continue; }
            if (c == ']') { inside = false; continue; }
            if (!inside) result.Add(c);
        }

        return new string(result.ToArray());
    }
}