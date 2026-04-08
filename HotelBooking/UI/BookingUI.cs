using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class BookingUI
    {
        private readonly IBookingService _bookingService;
        private readonly IGuestService _guestService;

        public BookingUI(IBookingService bookingService, IGuestService guestService)
        {
            _bookingService = bookingService;
            _guestService = guestService;
        }

        public void Create()
        {
            // 1. Hämta alla gäster
            var guests = _guestService.GetAllGuests();

            // 2. Låt användaren välja gäst
            var guestChoices = guests
                .Select(g => $"{g.Id}. {g.FullName} – {g.Email} – {g.PhoneNumber}")
                .ToList();

            guestChoices.Add("Skapa ny gäst");

            var selectedGuest = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [yellow]gäst[/]:")
                    .AddChoices(guestChoices)
            );

            int guestId;

            // 3. Om användaren vill skapa ny gäst
            if (selectedGuest == "Skapa ny gäst")
            {
                string fullName = AnsiConsole.Ask<string>("Fullständigt namn:");
                string email = AnsiConsole.Ask<string>("E-post:");
                string phone = AnsiConsole.Ask<string>("Telefonnummer:");

                var newGuest = _guestService.CreateGuest(fullName, email, phone);
                guestId = newGuest.Id;

                AnsiConsole.MarkupLine($"[green]Ny gäst skapad![/] ID: {guestId}");
            }
            else
            {
                guestId = int.Parse(selectedGuest.Split('.')[0]);
            }

            // 4. Välj datum via kalender
            var checkIn = ShowCalendarAndPickDate(DateTime.Today, DateTime.Today);
            var checkOut = ShowCalendarAndPickDate(checkIn.AddDays(1), checkIn.AddDays(1));

            // 5. Antal gäster (behövs för att filtrera rum)
            int guestsCount = AnsiConsole.Ask<int>("Antal gäster:");

            // 6. Hämta lediga rum baserat på datum + antal gäster
            var availableRooms = _bookingService
                .GetAvailableRooms(checkIn, checkOut, guestsCount);

            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga lediga rum för dessa datum.[/]");
                Console.ReadKey();
                return;
            }

            // 7. Låt användaren välja rum
            var roomChoices = availableRooms
                .Select(r => $"{r.Id}. Rum {r.RoomNumber} – {r.RoomType} – {r.PricePerNight} kr/natt – Max {r.MaxGuests} gäster")
                .ToList();

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [yellow]ledigt rum[/]:")
                    .AddChoices(roomChoices)
            );

            int roomId = int.Parse(selectedRoom.Split('.')[0]);
            var room = availableRooms.First(r => r.Id == roomId);

            // 8. Kontrollera att antal gäster får plats i rummet
            if (guestsCount > room.MaxGuests)
            {
                AnsiConsole.MarkupLine("[red]För många gäster för detta rum.[/]");
                Console.ReadKey();
                return;
            }

            // 9. Beräkna pris
            int nights = (checkOut - checkIn).Days;
            decimal totalPrice = nights * room.PricePerNight;

            AnsiConsole.MarkupLine($@"
[green]Prisberäkning[/]
Pris per natt: {room.PricePerNight} kr
Antal nätter: {nights}
Totalpris: [yellow]{totalPrice} kr[/]
");

            // 10. Skapa bokningen
            var booking = _bookingService.CreateBooking(
                guestId,
                roomId,
                checkIn,
                checkOut,
                guestsCount
            );

            AnsiConsole.MarkupLine($"[green]Bokning skapad![/] ID: {booking.Id}");
            Console.ReadKey();
        }

        // --- Kalenderfunktioner (UI-ansvar, inte service) ---

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

        public void Restore()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/] att återställa:");

            bool success = _bookingService.Restore(id);

            if (success)
                AnsiConsole.MarkupLine("[green]Bokningen är nu återställt![/]");
            else
                AnsiConsole.MarkupLine("[red]Bokningen hittades inte.[/]");

            Console.ReadKey();

        }

        public void SoftDelete()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/] att ta bort:");

            bool success = _bookingService.SoftDelete(id);

            if (success)
                AnsiConsole.MarkupLine("[yellow]Bokningen är nu markerat som ur funktion.[/]");
            else
                AnsiConsole.MarkupLine("[red]Bokningen hittades inte.[/]");

            Console.ReadKey();

        }

        public void Update()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/] att uppdatera:");

            var booking = _bookingService.GetBooking(id);
            if (booking == null)
            {
                AnsiConsole.MarkupLine("[red]Bokningen hittades inte.[/]");
                Console.ReadKey();
                return;
            }

            // Visa nuvarande värden
            AnsiConsole.MarkupLine($@"
[green]Nuvarande värden:[/]
Gäst-ID: {booking.GuestId}
Rum-ID: {booking.RoomId}
Från: {booking.FromDate:yyyy-MM-dd}
Till: {booking.ToDate:yyyy-MM-dd}
Antal gäster: {booking.NumberOfGuests}
");

            // Fråga efter nya värden (tomt = behåll)
            string newGuest = AnsiConsole.Ask<string>($"Ny gäst-ID (tomt = {booking.GuestId}):");
            string newRoom = AnsiConsole.Ask<string>($"Nytt rum-ID (tomt = {booking.RoomId}):");
            string newFrom = AnsiConsole.Ask<string>($"Nytt från-datum (yyyy-MM-dd, tomt = {booking.FromDate:yyyy-MM-dd}):");
            string newTo = AnsiConsole.Ask<string>($"Nytt till-datum (yyyy-MM-dd, tomt = {booking.ToDate:yyyy-MM-dd}):");
            string newGuests = AnsiConsole.Ask<string>($"Nytt antal gäster (tomt = {booking.NumberOfGuests}):");

            // Konvertera bara om användaren skrev något
            int? guestId = string.IsNullOrWhiteSpace(newGuest) ? null : int.Parse(newGuest);
            int? roomId = string.IsNullOrWhiteSpace(newRoom) ? null : int.Parse(newRoom);
            DateTime? fromDate = string.IsNullOrWhiteSpace(newFrom) ? null : DateTime.Parse(newFrom);
            DateTime? toDate = string.IsNullOrWhiteSpace(newTo) ? null : DateTime.Parse(newTo);
            int? guests = string.IsNullOrWhiteSpace(newGuests) ? null : int.Parse(newGuests);

            bool success = _bookingService.UpdateBooking(id, guestId, fromDate, toDate, guests);

            if (success)
                AnsiConsole.MarkupLine("[green]Bokningen uppdaterades![/]");
            else
                AnsiConsole.MarkupLine("[red]Uppdatering misslyckades.[/]");

            Console.ReadKey();
        }

        public void ShowAll()
        {
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Gäst-ID[/]");
            table.AddColumn("[yellow]Rum-ID[/]");
            table.AddColumn("[yellow]Från[/]");
            table.AddColumn("[yellow]Till[/]");
            table.AddColumn("[yellow]Gäster[/]");
            table.AddColumn("[yellow]Bokad[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var b in bookings)
            {
                table.AddRow(
                    b.Id.ToString(),
                    b.GuestId.ToString(),
                    b.RoomId.ToString(),
                    b.FromDate.ToString("yyyy-MM-dd"),
                    b.ToDate.ToString("yyyy-MM-dd"),
                    b.NumberOfGuests.ToString(),
                    b.BookingDate.ToString("yyyy-MM-dd"),
                    b.IsDeleted ? "[red]Raderad[/]" : "[green]Aktiv[/]"
                );
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }



        public void ShowDetails()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/]:");

            var booking = _bookingService.GetBooking(id);

            if (booking == null)
            {
                AnsiConsole.MarkupLine("[red]Bokningen hittades inte.[/]");
                Console.ReadKey();
                return;
            }

            var panel = new Panel($@"
[white]ID:[/] {booking.Id}
[white]Gäst-ID:[/] {booking.GuestId}
[white]Rum-ID:[/] {booking.RoomId}
[white]Från:[/] {booking.FromDate:yyyy-MM-dd}
[white]Till:[/] {booking.ToDate:yyyy-MM-dd}
[white]Antal gäster:[/] {booking.NumberOfGuests}
[white]Bokad:[/] {booking.BookingDate:yyyy-MM-dd HH:mm}
[white]Status:[/] {(booking.IsDeleted ? "[red]Raderad[/]" : "[green]Aktiv[/]")}
")
            {
                Header = new PanelHeader("[green]Bokningsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            Console.ReadKey();
        }
    }
}