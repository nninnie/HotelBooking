using HotelBooking.Models;
using HotelBooking.Services;
using HotelBooking.UI;
using Spectre.Console;
using Spectre.Console.Extensions;

public class BookingMenu : EscUI
{
    private readonly BookingService _bookingService;
    private readonly GuestService _guestService;

    public BookingMenu(BookingService bookingService, GuestService guestService)
    {
        _bookingService = bookingService;
        _guestService = guestService;
    }

    public void Show()
    {
        var guest = AskGuestSelection();
        var checkIn = AskCheckInDate();
        var checkOut = AskCheckOutDate(checkIn);
        var guests = AskGuestCount();

        var rooms = _bookingService.GetAvailableRooms(checkIn, checkOut, guests);

        var room = AskRoomSelection(rooms);

        _bookingService.CreateBooking(guest.Id, room.Id, checkIn, checkOut, guests);

        ShowConfirmation(room, checkIn, checkOut, guests);

        WaitForEsc();
    }

    private Guest AskGuestSelection()
    {
        var guests = _guestService.GetAllGuests();

        var prompt = new SelectionPrompt<Guest>()
            .Title("Välj gäst:")
            .PageSize(10)
            .UseConverter(g => $"{g.FullName} ({g.Email})")
            .AddChoices(guests);

        return AnsiConsole.Prompt(prompt);
    }

    private DateTime ShowCalendarAndPickDate(DateTime startDate, DateTime minDate)
    {
        DateTime selectedDate = startDate;

        while (true)
        {
            AnsiConsole.Clear();
            RenderCalendar(selectedDate, minDate);

            var keyInfo = AnsiConsole.Console.Input.ReadKey(true);
            if (keyInfo == null)
                continue;

            var key = keyInfo.Value.Key;

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
                    if (selectedDate >= minDate)
                        return selectedDate;
                    break;

                case ConsoleKey.Escape:
                    return minDate;
            }

            if (selectedDate < minDate)
                selectedDate = minDate;
        }
    }

    private void RenderCalendar(DateTime selectedDate, DateTime minDate)
    {
        var sw = new StringWriter();

        sw.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
        sw.WriteLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
        sw.WriteLine("─────────────────────────────────");

        DateTime firstDay = new DateTime(selectedDate.Year, selectedDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
        int startDay = (int)firstDay.DayOfWeek;
        startDay = (startDay == 0) ? 6 : startDay - 1;

        for (int i = 0; i < startDay; i++)
            sw.Write("     ");

        for (int day = 1; day <= daysInMonth; day++)
        {
            DateTime current = new DateTime(selectedDate.Year, selectedDate.Month, day);

            if (current == selectedDate)
                sw.Write($"[green]{day,2}[/]   ");
            else if (current < minDate)
                sw.Write($"[grey]{day,2}[/]   ");
            else
                sw.Write($"{day,2}   ");

            if ((startDay + day) % 7 == 0)
                sw.WriteLine();
        }

        var panel = new Panel(sw.ToString())
        {
            Border = BoxBorder.Double,
            Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
        };

        AnsiConsole.Write(panel);
        AnsiConsole.MarkupLine("\nNavigera med [blue]piltangenter[/], välj med [green]Enter[/].");
    }

    private DateTime AskCheckInDate()
    {
        DateTime today = DateTime.Today;
        return ShowCalendarAndPickDate(today, today);
    }

    private DateTime AskCheckOutDate(DateTime checkIn)
    {
        DateTime earliest = checkIn.AddDays(1);
        return ShowCalendarAndPickDate(earliest, earliest);
    }

    private int AskGuestCount()
    {
        var prompt = new TextPrompt<int>("Hur många gäster ska bo i rummet?")
            .DefaultValue(1)
            .Validate(count =>
            {
                if (count < 1)
                    return ValidationResult.Error("Antal gäster måste vara minst 1.");
                if (count > 10)
                    return ValidationResult.Error("Antal gäster kan inte överstiga 10.");
                return ValidationResult.Success();
            });

        return AnsiConsole.Prompt(prompt);
    }

    private Room AskRoomSelection(List<Room> rooms)
    {
        var prompt = new SelectionPrompt<Room>()
            .Title("Välj rum:")
            .PageSize(10)
            .UseConverter(r => $"{r.RoomNumber} - {r.RoomType} (Max {r.MaxGuests} gäster) - {r.PricePerNight:C}")
            .AddChoices(rooms);

        return AnsiConsole.Prompt(prompt);
    }

    private void ShowConfirmation(Room room, DateTime checkIn, DateTime checkOut, int guests)
    {
        var bookingConfirmation = new Panel(
            $"[green]Bokning skapad![/]\n\n" +
            $"[bold]Rum:[/] {room.RoomNumber} - {room.RoomType}\n" +
            $"[bold]Incheckning:[/] {checkIn:yyyy-MM-dd}\n" +
            $"[bold]Utcheckning:[/] {checkOut:yyyy-MM-dd}\n" +
            $"[bold]Antal gäster:[/] {guests}\n" +
            $"[bold]Pris per natt:[/] {room.PricePerNight:C}"
        )
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1),
            Header = new PanelHeader("[green]Bekräftelse[/]", Justify.Center)
        };

        AnsiConsole.Write(bookingConfirmation);

        WaitForEsc();
    }
}