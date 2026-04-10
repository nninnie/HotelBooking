using Spectre.Console;
using System.Globalization;

public static class CalenderPicker
{
    public static DateTime? SelectDate()
    {
        DateTime today = DateTime.Today;
        DateTime maxDate = today.AddYears(2); // Ska inte kunna bläddra flera år framåt i kalendern
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

            // Stoppa användaren från att gå bakåt i tiden
            if (selectedDate < today)
                selectedDate = today;

            // Stoppa användaren från att gå för långt fram
            if (selectedDate > maxDate)
                selectedDate = maxDate;

            // Fix för dagar som inte finns i månaden
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            if (selectedDate.Day > daysInMonth)
                selectedDate = new DateTime(selectedDate.Year, selectedDate.Month, daysInMonth);
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
        AnsiConsole.MarkupLine("\nAnvänd piltangenter [blue]◄ ▲ ► ▼[/] för att navigera.");
        AnsiConsole.MarkupLine("Tryck [green]Enter[/] för att välja eller [red]ESC[/] för att avbryta.");
    }
}