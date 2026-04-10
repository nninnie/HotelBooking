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

        public BookingUI(IBookingService bookingService, IGuestService guestService, IRoomService roomService)
        {
            _bookingService = bookingService;
            _guestService = guestService;
            _roomService = roomService;
        }

        // SKAPA NY BOKNING
        public void Create()
        {
            CurrentHeader = "SKAPA BOKNING";

            // Välj eller skapa kund
            var guestId = BookingPrompts.SelectOrCreateGuest(this, _guestService);
            if (guestId == null) return;

            // Datum
            AnsiConsole.MarkupLine("[green]Välj incheckningsdatum:[/]");
            DateTime? checkIn = null;

            while (true)
            {
                AnsiConsole.MarkupLine("[green]Välj incheckningsdatum:[/]");
                checkIn = CalenderPicker.SelectDate();

                if (checkIn == null)
                    return;

                // VALIDATION
                if (checkIn.Value.Date < DateTime.Today)
                {
                    BookingRenderer.ShowError("Incheckningsdatum kan inte vara bakåt i tiden.");
                    continue;
                }

                break;
            }

            AnsiConsole.MarkupLine("[green]Välj utcheckningsdatum:[/]");
            DateTime? checkOut = null;

            while (true)
            {
                checkOut = CalenderPicker.SelectDate();
                if (checkOut == null) return;

                if (checkOut > checkIn)
                    break;

                BookingRenderer.ShowError("Utcheckningsdatum måste vara efter incheckningsdatum!");
                AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att fortsätta...[/]");
                Console.ReadKey(true);
            }

            // Antal gäster
            var guestsCount = BookingPrompts.AskGuestCount(this);
            if (guestsCount == null) return;

            // VALIDATION
            if (guestsCount <= 0)
            {
                BookingRenderer.ShowError("Antal gäster måste vara minst 1.");
                WaitForEsc();
                return;
            }

            // Lediga rum
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

            // Skapa bokning
            var booking = _bookingService.CreateBooking(
                guestId.Value,
                roomId.Value,
                checkIn.Value,
                checkOut.Value,
                guestsCount.Value
            );

            BookingRenderer.ShowSuccess($"Bokning skapad! ID: {booking.Id}");
            WaitForEsc();
        }

        // VISA ALLA
        public void ShowAll()
        {
            CurrentHeader = "ALLA BOKNINGAR";

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

            WaitForEsc();
        }

        // VISA DETALJER
        public void ShowDetails()
        {
            CurrentHeader = "BOKNINGSDETALJER";

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

            // VALIDATION
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
        public void SoftDelete()
        {
            CurrentHeader = "RADERA BOKNING";

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

            // VALIDATION
            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.SoftDelete(bookingId.Value);

            BookingRenderer.ShowResult(
                success,
                "Bokningen är nu markerad som raderad.",
                "Bokningen hittades inte."
            );

            WaitForEsc();
        }

        // RESTORE
        public void Restore()
        {
            CurrentHeader = "ÅTERSTÄLL BOKNING";

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

            // VALIDATION
            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
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
        public void Update()
        {
            CurrentHeader = "UPPDATERA BOKNING";

            var bookings = _bookingService.GetAll();

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            // VALIDATION
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

            // VALIDATION
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