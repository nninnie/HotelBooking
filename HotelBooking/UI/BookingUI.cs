using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

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

            // NYTT: välj båda datumen i ett steg
            var (checkIn, checkOut) = CalendarPicker.SelectDateRange();

            if (checkIn == null || checkOut == null)
                return;

            if (checkIn.Value.Date < DateTime.Today)
            {
                BookingRenderer.ShowError("Incheckningsdatum kan inte vara bakåt i tiden.");
                WaitForEsc();
                return;
            }

            if (checkOut.Value <= checkIn.Value)
            {
                BookingRenderer.ShowError("Utcheckningsdatum måste vara efter incheckningsdatum!");
                WaitForEsc();
                return;
            }

            var guestsCount = BookingPrompts.AskGuestCount(this);
            if (guestsCount == null || guestsCount <= 0)
            {
                BookingRenderer.ShowError("Antal gäster måste vara minst 1.");
                WaitForEsc();
                return;
            }

            var availableRooms = _bookingService.GetAvailableRooms(
                checkIn.Value,
                checkOut.Value,
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

            var booking = _bookingService.CreateBooking(
                guestId.Value,
                roomId.Value,
                checkIn.Value,
                checkOut.Value,
                guestsCount.Value
            );

            var room = _roomService.GetRoom(roomId.Value);

            if (room == null)
            {
                BookingRenderer.ShowError("Kunde inte hämta rumsinformation.");
                WaitForEsc();
                return;
            }

            int nights = (checkOut.Value - checkIn.Value).Days;
            decimal totalPrice = nights * room.PricePerNight;  

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Bokningen är klar![/]");
            AnsiConsole.MarkupLine($"Rum: [yellow]{room.RoomNumber}[/] ({room.RoomType})");
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

        // VISA DETALJER
        public void ShowDetails(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Visa detaljer");
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

            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
                WaitForEsc();
                return;
            }

            var booking = _bookingService.GetBooking(bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            BookingRenderer.ShowBookingDetails(booking, guest, room);
            WaitForEsc();
        }

        // SOFT DELETE
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

            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
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

            var bookings = _bookingService
                .GetAll()
                .Where(b => b.IsDeleted)
                .ToList();

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

            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
                WaitForEsc();
                return;
            }

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill avboka denna bokning?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Avbokning avbröts.");
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

            var booking = _bookingService.GetBooking(bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            var updates = BookingPrompts.AskBookingUpdates(this, booking);
            if (updates == null) return;

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
    }
}