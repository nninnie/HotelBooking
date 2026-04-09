using Spectre.Console;
using System.Globalization;

public static class CalenderPicker
{
    public static DateTime? SelectDate()
    {
        DateTime today = DateTime.Today;
        DateTime selectedDate = today; // Starta alltid på dagens datum

        while (true)
        {
            Console.Clear();
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

            // STOPPA användaren från att gå bakåt i tiden
            if (selectedDate < today)
                selectedDate = today;
        }
    }

    private static void RenderCalendar(DateTime selectedDate)
    {
        var calendarContent = new StringWriter();

        // Månadstitel
        calendarContent.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
        calendarContent.WriteLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
        calendarContent.WriteLine("─────────────────────────────────");

        DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
        int startDay = (int)firstDayOfMonth.DayOfWeek;
        startDay = (startDay == 0) ? 6 : startDay - 1; // Gör måndag till första dagen

        // Tomma rutor innan månadens första dag
        for (int i = 0; i < startDay; i++)
            calendarContent.Write("     ");

        // Dagar
        for (int day = 1; day <= daysInMonth; day++)
        {
            if (day == selectedDate.Day)
                calendarContent.Write($"[green]{day,2}[/]   "); // Markera vald dag
            else
                calendarContent.Write($"{day,2}   ");

            if ((startDay + day) % 7 == 0)
                calendarContent.WriteLine();
        }

        // Panel runt kalendern
        var panel = new Panel(calendarContent.ToString())
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
        };

        AnsiConsole.Write(panel);

        Console.WriteLine();
        AnsiConsole.MarkupLine("\nAnvänd piltangenter [blue]\u25C4 \u25B2 \u25BA \u25BC[/] för att navigera.");
        AnsiConsole.MarkupLine("Tryck [green]Enter[/] för att välja eller [red]ESC[/] för att avbryta.");
    }
}