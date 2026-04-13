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
                            Thread.Sleep(1200);
                        }
                        else
                        {
                            checkOut = cursor;
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

        sb.AppendLine($"[red]{cursor:MMMM yyyy}[/]".ToUpper());
        sb.AppendLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
        sb.AppendLine("───────────────────────────────");

        DateTime firstDay = new DateTime(cursor.Year, cursor.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(cursor.Year, cursor.Month);

        int startDay = (int)firstDay.DayOfWeek;
        startDay = (startDay == 0) ? 6 : startDay - 1;

        for (int i = 0; i < startDay; i++)
            sb.Append("     ");

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime current = new DateTime(cursor.Year, cursor.Month, day);

            bool isCursor = day == cursor.Day;
            bool isCheckIn = checkIn != null && current == checkIn.Value;
            bool isCheckOut = checkOut != null && current == checkOut.Value;
            bool isRange =
                checkIn != null &&
                checkOut != null &&
                current > checkIn.Value &&
                current < checkOut.Value;

            if (isCheckIn)
                sb.Append($"[bold green]{day,2}[/]  ");
            else if (isCheckOut)
                sb.Append($"[bold red]{day,2}[/]  ");
            else if (isRange)
                sb.Append($"[yellow]{day,2}[/]  ");
            else if (isCursor)
                sb.Append($"[cyan]{day,2}[/]  ");
            else
                sb.Append($"{day,2}  ");
        }

        var panel = new Panel(sb.ToString())
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader(
                selectingCheckIn ? "[green]Välj incheckning[/]" : "[red]Välj utcheckning[/]",
                Justify.Center)
        };

        AnsiConsole.Write(panel);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[grey]Piltangenter = navigera[/]");
        AnsiConsole.MarkupLine("[grey]Enter = välj datum[/]");
        AnsiConsole.MarkupLine("[grey]ESC = avbryt[/]");

        if (checkIn != null && selectingCheckIn == false)
        {
            AnsiConsole.MarkupLine($"[green]Incheckning:[/] [yellow]{checkIn:yyyy-MM-dd}[/]");
        }
    }
}