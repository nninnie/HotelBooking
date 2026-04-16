using Spectre.Console;
using System.Globalization;
using System.Text;

public static class CalendarPicker
{
    public static (DateTime? CheckIn, DateTime? CheckOut) SelectDateRange()
    {
        DateTime today = DateTime.Today;
        DateTime maxDate = today.AddYears(2);

        DateTime cursor = today;
        DateTime? checkIn = null;
        DateTime? checkOut = null;

        bool selectingCheckIn = true;

        while (true)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", selectingCheckIn ? "Välj incheckning" : "Välj utcheckning");

            AnsiConsole.WriteLine();
            RenderCalendar(cursor, checkIn, checkOut, selectingCheckIn);

            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.RightArrow: cursor = cursor.AddDays(1); break;
                case ConsoleKey.LeftArrow: cursor = cursor.AddDays(-1); break;
                case ConsoleKey.UpArrow: cursor = cursor.AddDays(-7); break;
                case ConsoleKey.DownArrow: cursor = cursor.AddDays(7); break;

                case ConsoleKey.Enter:
                    if (selectingCheckIn)
                    {
                        checkIn = cursor;
                        selectingCheckIn = false;
                    }
                    else
                    {
                        if (cursor <= checkIn)
                        {
                            AnsiConsole.MarkupLine("[red]Utcheckning måste vara efter incheckning![/]");
                            Thread.Sleep(500);
                        }
                        else
                        {
                            checkOut = cursor;

                            // Visa vald utcheckning
                            AnsiConsole.MarkupLine($"[yellow]Utcheckning vald: [/][green]{checkOut:yyyy-MM-dd}[/]");
                            Thread.Sleep(100);

                            // Visa intervallet innan vi går vidare
                            AnsiConsole.Clear();
                            BannerUI.ShowLogo();
                            BannerUI.PrintHeader("Hantera bokningar", "Valt datumintervall");

                            RenderCalendar(cursor, checkIn, checkOut, selectingCheckIn: false);

                            Thread.Sleep(500);

                            return (checkIn, checkOut);
                        }
                    }
                    break;

                case ConsoleKey.Escape:
                    return (null, null);
            }

            if (cursor < today) cursor = today;
            if (cursor > maxDate) cursor = maxDate;

            int daysInMonth = DateTime.DaysInMonth(cursor.Year, cursor.Month);
            if (cursor.Day > daysInMonth)
                cursor = new DateTime(cursor.Year, cursor.Month, daysInMonth);
        }
    }

    public static void RenderCalendar(DateTime cursor, DateTime? checkIn, DateTime? checkOut, bool selectingCheckIn)
    {
        var sb = new StringBuilder();

        // Titel
        sb.AppendLine($"[blue]{cursor:MMMM yyyy}[/]".ToUpper());
        sb.AppendLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
        sb.AppendLine("───────────────────────────────");

        DateTime firstDay = new DateTime(cursor.Year, cursor.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(cursor.Year, cursor.Month);

        int startDay = ((int)firstDay.DayOfWeek + 6) % 7;

        // Tomma rutor innan månadens start
        for (int i = 0; i < startDay; i++)
            sb.Append("     "); 

        // Dagar
        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime current = new DateTime(cursor.Year, cursor.Month, day);

            bool isCursor = current == cursor;
            bool isCheckIn = checkIn != null && current == checkIn.Value;
            bool isCheckOut = checkOut != null && current == checkOut.Value;
            bool isRange =
                checkIn != null &&
                checkOut != null &&
                current > checkIn.Value &&
                current < checkOut.Value;

            bool isPast = current < DateTime.Today;

            string text;

            if (isCheckIn)
                text = $"[bold green]{day,2}[/]";
            else if (isCheckOut)
                text = $"[bold green]{day,2}[/]"; // utcheckning också grön
            else if (isRange)
                text = $"[green3]{day,2}[/]";     // ljusgrön
            else if (isCursor)
                text = selectingCheckIn ? $"[orange1]{day,2}[/]" : $"[green]{day,2}[/]";
            else if (isPast)
                text = $"[grey]{day,2}[/]";
            else
                text = $"{day,2}";

            sb.Append($"{text}   ");

            if ((startDay + (day - 1)) % 7 == 6)
                sb.AppendLine();
        }

        // Panel
        var panel = new Panel(sb.ToString())
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader(
                selectingCheckIn ? "[green]Välj incheckning[/]" : "[green]Välj utcheckning[/]",
                Justify.Center)
        };

        AnsiConsole.Write(panel);

        // Instruktioner
        AnsiConsole.MarkupLine("[grey]Piltangenter = navigera[/]");
        AnsiConsole.MarkupLine("[grey]Enter = välj datum[/]");
        AnsiConsole.MarkupLine("[grey]ESC = avbryt[/]");

        // Visa valda datum
        if (checkIn != null)
            AnsiConsole.MarkupLine($"\n[yellow]Incheckning vald:[/] [green]{checkIn:yyyy-MM-dd}[/]");

        if (checkOut != null)
            AnsiConsole.MarkupLine($"[yellow]Utcheckning vald:[/] [green]{checkOut:yyyy-MM-dd}[/]");
    }
}
