using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HotelBooking.UI
{
    public class BookingUI : EscUI
    {
        private readonly IBookingService _bookingService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;
        private readonly InvoiceUI _invoiceUI;

        public BookingUI(IBookingService bookingService, IGuestService guestService, IRoomService roomService, InvoiceUI invoiceUI)
        {
            _bookingService = bookingService;
            _guestService = guestService;
            _roomService = roomService;
            _invoiceUI = invoiceUI;
        }

        // SKAPA NY BOKNING
        public void Create(string subtitle)
        {
            // HEADER
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Skapa ny bokning");
            AnsiConsole.WriteLine();

            var guestId = BookingPrompts.SelectOrCreateGuest(this, _guestService);
            if (guestId == null) return;

            // Välj datumintervall
            var (checkInDate, checkOutDate) = CalendarPicker.SelectDateRange();

            if (checkInDate == null || checkOutDate == null)
                return;

            if (checkInDate.Value.Date < DateTime.Today)
            {
                BookingRenderer.ShowError("Incheckningsdatum kan inte vara bakåt i tiden.");
                WaitForEsc();
                return;
            }

            if (checkOutDate.Value <= checkInDate.Value)
            {
                BookingRenderer.ShowError("Utcheckningsdatum måste vara efter incheckningsdatum!");
                WaitForEsc();
                return;
            }

            // Sätt tiderna här
            var checkIn = checkInDate.Value.Date.AddHours(15);   // 15:00
            var checkOut = checkOutDate.Value.Date.AddHours(11); // 11:00

            var guestsCount = BookingPrompts.AskGuestCount(this);
            if (guestsCount == null || guestsCount <= 0)
            {
                BookingRenderer.ShowError("Antal gäster måste vara minst 1.");
                WaitForEsc();
                return;
            }

            var availableRooms = _bookingService.GetAvailableRooms(
                checkIn,
                checkOut,
                guestsCount.Value
            );

            if (!availableRooms.Any())
            {
                BookingRenderer.ShowError("Inga lediga rum för dessa datum.");
                WaitForEsc();
                return;
            }

            var roomId = BookingPrompts.SelectRoom(this, availableRooms);
            if (roomId == null) return;

            // Skapa bokningen med tider
            var booking = _bookingService.CreateBooking(
                guestId.Value,
                roomId.Value,
                checkIn,
                checkOut,
                guestsCount.Value
            );

            var room = _roomService.GetRoom(roomId.Value);

            if (room == null)
            {
                BookingRenderer.ShowError("Kunde inte hämta rumsinformation.");
                WaitForEsc();
                return;
            }

            int nights = (checkOut - checkIn).Days;
            decimal totalPrice = nights * room.PricePerNight;

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Bokningen är klar![/]");
            AnsiConsole.MarkupLine($"Rum: [yellow]{room.RoomNumber}[/] ({room.RoomType})");
            AnsiConsole.MarkupLine($"Incheckning: [yellow]{checkIn:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"Utcheckning: [yellow]{checkOut:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"Antal nätter: [yellow]{nights}[/]");
            AnsiConsole.MarkupLine($"Pris per natt: [yellow]{room.PricePerNight} kr[/]");
            AnsiConsole.MarkupLine($"Totalpris: [bold yellow]{totalPrice} kr[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("[blue]Nyckelkort skapas...[/]");
            AnsiConsole.MarkupLine($"[green]✔ Nyckelkort till rum {room.RoomNumber} överlämnat till kunden.[/]");
            WaitForEsc();
        }


        // VISA ALLA
        public void ShowAll(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Visa alla");
            AnsiConsole.WriteLine();

            var bookings = _bookingService
                .GetAll();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            BookingRenderer.
                ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            WaitForEsc();
        }

        public void ShowDetails(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);
            AnsiConsole.WriteLine();

            // Hämta alla bokningar
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            // Visa LISTAN
            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            // Fråga efter ID
            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            var booking = _bookingService.GetBooking(bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);
            AnsiConsole.WriteLine();

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            // Visa DETALJER 
            int nights = (booking.ToDate - booking.FromDate).Days;
            if (nights < 1) nights = 1;

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            WaitForEsc();
        }


        public void SoftDelete(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Avboka");
            AnsiConsole.WriteLine();

            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            // ⭐ Kontrollera att bokningen finns i listan du visade
            var booking = bookings.FirstOrDefault(b => b.Id == bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            bool confirm = BookingPrompts.Confirm("\nÄr du säker att du vill ta bort denna bokning?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Avbokning avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.SoftDelete(bookingId.Value);

            BookingRenderer.ShowResult(
                success,
                "Bokningen är nu markerad som avbokad.",
                "Bokningen hittades inte."
            );

            WaitForEsc();
        }

        // RESTORE
        public void Restore(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Återställ bokning");
            AnsiConsole.WriteLine();

            var bookings = _bookingService.GetDeleted().ToList();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            var booking = bookings.FirstOrDefault(b => b.Id == bookingId.Value);

            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", $"Återställ boknings ID: {bookingId.Value}");
            AnsiConsole.WriteLine();

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            // Beräkna nätter
            int nights = (booking.ToDate - booking.FromDate).Days;
            if (nights < 1) nights = 1;

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            // BEKRÄFTA EFTER PANELEN
            bool confirm = BookingPrompts.Confirm("\nÄr du säker att du vill återställa denna bokning?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Återställning avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.Restore(bookingId.Value);

            BookingRenderer.ShowResult(
                success,
                "Bokningen är nu återställd!",
                "Bokningen hittades inte."
            );

            WaitForEsc();
        }


        // UPDATE
        public void Update(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Uppdatera bokning");
            AnsiConsole.WriteLine();

            var bookings = _bookingService.GetAll();

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", $"Uppdatera boknings ID: {bookingId.Value}");
            AnsiConsole.WriteLine();

            var booking = _bookingService.GetBooking(bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            // Beräkna nätter
            int nights = (booking.ToDate - booking.FromDate).Days;
            if (nights < 1) nights = 1;

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            // HÄR EFTER PANELEN FRÅGAR VI EFTER ÄNDRINGAR
            var updates = BookingPrompts.AskBookingUpdates(this, booking);
            if (updates == null) return;

            // Fallback till befintliga datum om användaren lämnar tomt
            var from = updates.FromDate ?? booking.FromDate;
            var to = updates.ToDate ?? booking.ToDate;

            // Lägg på standardtider
            from = from.Date.AddHours(15);
            to = to.Date.AddHours(11);

            // Spara tillbaka
            updates.FromDate = from;
            updates.ToDate = to;

            if (updates.FromDate >= updates.ToDate)
            {
                BookingRenderer.ShowError("Utcheckningsdatum måste vara efter incheckningsdatum.");
                WaitForEsc();
                return;
            }

            if (updates.NumberOfGuests <= 0)
            {
                BookingRenderer.ShowError("Antal gäster måste vara minst 1.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.UpdateBooking(
                booking.Id,
                updates.RoomId,
                updates.FromDate,
                updates.ToDate,
                updates.NumberOfGuests
            );


            BookingRenderer.ShowResult(
                success,
                "Bokningen uppdaterades!",
                "Uppdatering misslyckades."
            );

            WaitForEsc();
        }

        private Panel BuildBookingPanel(Booking booking, Guest? guest, Room? room)
        {
            int nights = (booking.ToDate - booking.FromDate).Days;
            if (nights < 1) nights = 1;

            string status = booking.IsCancelled
                ? "[red]● Avbokad[/]"
                : booking.IsActive
                    ? "[green]● Aktiv[/]"
                    : "[yellow]● Avslutad[/]";

            return new Panel($@"[grey]Boknings-ID:[/] [white]{booking.Id}[/]

[bold dodgerblue1]Bokning[/]
[grey]Boknings-ID:[/] [white]{booking.Id}[/]
[grey]Skapad:[/] [white]{booking.BookingDate:yyyy-MM-dd HH:mm}[/]
[grey]Status:[/] {status}

[bold dodgerblue1]Gäst[/]
[grey]Namn:[/] [white]{guest?.FirstName} {guest?.LastName}[/]
[grey]E-post:[/] [white]{guest?.Email}[/]
[grey]Telefon:[/] [white]{guest?.PhoneNumber}[/]

[bold dodgerblue1]Rum[/]
[grey]Rum:[/] [white]{room?.RoomNumber} ({room?.RoomType})[/]
[grey]Pris per natt:[/] [white]{room?.PricePerNight} kr[/]

[bold dodgerblue1]Datum[/]
[grey]Incheckning:[/] [white]{booking.FromDate:yyyy-MM-dd HH:mm}[/]
[grey]Utcheckning:[/] [white]{booking.ToDate:yyyy-MM-dd HH:mm}[/]
[grey]Antal nätter:[/] [white]{nights}[/]

[bold dodgerblue1]Pris[/]
[grey]Totalpris:[/] [bold white]{booking.TotalPrice} kr[/]
")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };
        }

        public void CheckIn(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);

            var bookings = _bookingService.GetAll()
                .Where(b => !b.IsCheckedIn && !b.IsCompleted)
                .ToList();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar att checka in.");
                WaitForEsc();
                return;
            }

            var bookingId = BookingPrompts.SelectBooking(this, bookings, subtitle);
            if (bookingId == null)
                return;

            if (_bookingService.CheckIn(bookingId.Value))
                AnsiConsole.MarkupLine("[green]✔ Gästen är nu incheckad.[/]");
            else
                BookingRenderer.ShowError("Kunde inte checka in gästen.");

            WaitForEsc();
        }


        public void CheckOut(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);

            var bookings = _bookingService.GetAll()
                .Where(b => b.IsCheckedIn)
                .ToList();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga gäster är incheckade.");
                WaitForEsc();
                return;
            }

            // RÄTT METOD
            var bookingId = BookingPrompts.SelectCheckedInBooking(this, _bookingService, subtitle);
            if (bookingId == null)
                return;

            bool? pay = AskBoolOrEsc("Vill du betala fakturan nu?", defaultValue: false);
            if (pay == null)
                return;

            if (_bookingService.CheckOut(bookingId.Value, pay.Value))
            {
                AnsiConsole.MarkupLine("[green]✔ Gäst utcheckad.[/]");
                WaitForEsc();
            }
            else
            {
                BookingRenderer.ShowError("Kunde inte checka ut gästen.");
                WaitForEsc();
            }
        }

    }
}
