using Spectre.Console;
using System.Globalization;
using System.Text;

public static class CalendarPicker
{
    public static DateTime? SelectDate()
    {
        DateTime today = DateTime.Today;
        DateTime maxDate = today.AddYears(2);
        DateTime selectedDate = today;

        while (true)
        {
            AnsiConsole.Clear();
            RenderCalendar(selectedDate);

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.RightArrow:
                    selectedDate = selectedDate.AddDays(1);
                    break;

                case ConsoleKey.LeftArrow:
                    selectedDate = selectedDate.AddDays(-1);
                    break;

                case ConsoleKey.UpArrow:
                    selectedDate = selectedDate.AddDays(-7);
                    break;

                case ConsoleKey.DownArrow:
                    selectedDate = selectedDate.AddDays(7);
                    break;

                case ConsoleKey.Enter:
                    return selectedDate;

                case ConsoleKey.Escape:
                    return null;
            }

            if (selectedDate < today)
                selectedDate = today;

            if (selectedDate > maxDate)
                selectedDate = maxDate;

            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            if (selectedDate.Day > daysInMonth)
                selectedDate = new DateTime(selectedDate.Year, selectedDate.Month, daysInMonth);
        }
    }

    private static void RenderCalendar(DateTime selectedDate)
    {
        var sb = new StringBuilder();

        // Titel
        sb.AppendLine($"[red]{selectedDate:MMMM yyyy}[/]".ToUpper());
        sb.AppendLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
        sb.AppendLine("─────────────────────────────────");

        DateTime firstDay = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);

        int startDay = (int)firstDay.DayOfWeek;
        startDay = (startDay == 0) ? 6 : startDay - 1;

        // Tomma rutor
        for (int i = 0; i < startDay; i++)
            sb.Append("     ");

        // Dagar
        for (int day = 1; day <= daysInMonth; day++)
        {
            if (day == selectedDate.Day)
                sb.Append($"[green]{day,2}[/]   ");
            else
                sb.Append($"{day,2}   ");

            if ((startDay + day) % 7 == 0)
                sb.AppendLine();
        }

        var panel = new Panel(sb.ToString())
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
        };

        AnsiConsole.Write(panel);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Använd piltangenter för att navigera.[/]");
        AnsiConsole.MarkupLine("[grey]Enter = välj, ESC = avbryt[/]");
    }
}