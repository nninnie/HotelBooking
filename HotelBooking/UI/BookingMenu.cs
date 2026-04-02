using HotelBooking.Models;
using HotelBooking.Services;
using Spectre.Console;
using Spectre.Console.Extensions;

public class BookingMenu
{
    // BookingService används för att skapa bokningar och hämta lediga rum
    private readonly BookingService _bookingService;

    // GuestService används för att hämta alla gäster så användaren kan välja en
    private readonly GuestService _guestService;

    // Konstruktor: BookingMenu behöver BookingService och GuestService för att fungera
    public BookingMenu(BookingService bookingService, GuestService guestService)
    {
        _bookingService = bookingService;
        _guestService = guestService;
    }

    // Show() är huvudflödet för att skapa en bokning
    public void Show()
    {
        // 1. Låt användaren välja vilken gäst som ska boka
        var guest = AskGuestSelection();

        // 2. Välj incheckningsdatum via kalender
        var checkIn = AskCheckInDate();

        // 3. Välj utcheckningsdatum via kalender
        var checkOut = AskCheckOutDate(checkIn);

        // 4. Fråga hur många gäster som ska bo i rummet
        var guests = AskGuestCount();

        // 5. Hämta lediga rum baserat på datum och antal gäster
        var rooms = _bookingService.GetAvailableRooms(checkIn, checkOut, guests);

        // 6. Låt användaren välja ett av de lediga rummen
        var room = AskRoomSelection(rooms);

        // 7. Skapa bokningen i databasen via BookingService
        _bookingService.CreateBooking(room.Id, guest.Id, checkIn, checkOut, guests);

        // 8. Visa en snygg bekräftelse
        ShowConfirmation(room, checkIn, checkOut, guests);
    }

    // Låter användaren välja en gäst från en lista
    private Guest AskGuestSelection()
    {
        // 1. Hämta alla gäster
        var guests = _guestService.GetAllGuests();

        // 2. Skapa en SelectionPrompt<Guest>
        var prompt = new SelectionPrompt<Guest>()
            .Title("Välj gäst:")
            .PageSize(10)
            .UseConverter(g => $"{g.FullName} ({g.Email})")
            .AddChoices(guests);

        // 3. Returnera det valda objektet
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
                    return minDate; // eller throw om du vill avbryta
            }

            // Blockera datum bakåt
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

    // Visar kalender och returnerar valt incheckningsdatum
    private DateTime AskCheckInDate()
    {
        DateTime today = DateTime.Today;
        return ShowCalendarAndPickDate(today, today);
    }

    // Visar kalender och returnerar utcheckningsdatum
    private DateTime AskCheckOutDate(DateTime checkIn)
    {
        DateTime earliest = checkIn.AddDays(1);
        return ShowCalendarAndPickDate(earliest, earliest);
    }

    // Frågar användaren hur många gäster som ska bo i rummet
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

    // Låter användaren välja ett rum från listan av lediga rum
    private Room AskRoomSelection(List<Room> rooms)
    {
        var selectedRoom = rooms.FirstOrDefault();

        var prompt = new SelectionPrompt<Room>()
            .Title("Välj rum:")
            .PageSize(10)
            .UseConverter(r => $"{r.RoomNumber} - {r.RoomType} (Max {r.MaxGuests} gäster) - {r.PricePerNight:C}")
            .AddChoices(rooms);

        return AnsiConsole.Prompt(prompt);
    }

    // Visar en bekräftelse efter att bokningen skapats
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

        AnsiConsole.MarkupLine("\n[grey]Tryck Enter för att fortsätta...[/]");
        Console.ReadLine();
    }
}