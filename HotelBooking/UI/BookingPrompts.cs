using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    // En hjälparklass som samlar alla frågor (prompts)
    // som BookingUI behöver. Gör koden renare och mer SOLID.
    public static class BookingPrompts
    {
        // Fråga användaren om de vill skapa ny kund eller välja befintlig
        public static int? SelectOrCreateGuest(EscUI ui, IGuestService guestService)
        {
            var choice = ui.SelectWithEsc(
                new List<string>
                {
                    "Skapa bokning med NY kund",
                    "Skapa bokning med BEFINTLIG kund",
                    "Avbryt"
                },
                s => s
            );

            if (choice == null || choice == "Avbryt")
                return null;

            if (choice == "Skapa bokning med NY kund")
                return CreateGuest(ui, guestService);

            return SelectExistingGuest(ui, guestService);
        }

        // Skapa en ny kund
        public static int? CreateGuest(EscUI ui, IGuestService guestService)
        {
            string? fullName = ui.AskWithEsc("Fullständigt namn:");
            if (fullName == null) return null;

            string? phone = ui.AskWithEsc("Telefonnummer:");
            if (phone == null) return null;

            string? email = ui.AskWithEsc("Email:");
            if (email == null) return null;

            var guest = guestService.CreateGuest(fullName, phone, email);
            return guest.Id;
        }

        // Välj en befintlig kund
        public static int? SelectExistingGuest(EscUI ui, IGuestService guestService)
        {
            var guests = guestService.GetAllGuests();

            if (!guests.Any())
                return null;

            var selected = ui.SelectWithEsc(
                guests.ToList(),
                g => $"{g.FullName} – {g.Email} – {g.PhoneNumber}"
            );

            return selected?.Id;
        }

        // Fråga efter antal gäster
        public static int? AskGuestCount(EscUI ui)
        {
            string? input = ui.AskWithEsc("Antal gäster:");
            if (input == null) return null;

            if (!int.TryParse(input, out int count))
                return null;

            return count;
        }

        // Välj rum från en lista
        public static int? SelectRoom(EscUI ui, IEnumerable<Room> rooms)
        {
            var selected = ui.SelectWithEsc(
                rooms.ToList(),
                r => $"{r.RoomNumber} – {r.RoomType} – {r.PricePerNight} kr/natt (Max {r.MaxGuests})"
            );

            return selected?.Id;
        }


        // Fråga efter boknings-ID
        public static int? AskBookingId(EscUI ui)
        {
            string? input = ui.AskWithEsc("Ange boknings-ID:");
            if (input == null) return null;

            if (!int.TryParse(input, out int id))
                return null;

            return id;
        }

        // Modell för uppdateringsvärden
        public class BookingUpdateModel
        {
            public int? GuestId { get; set; }
            public int? RoomId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? NumberOfGuests { get; set; }
        }

        // Fråga efter uppdateringar av en bokning
        public static BookingUpdateModel? AskBookingUpdates(EscUI ui, Booking booking)
        {
            AnsiConsole.MarkupLine("[green]Lämna tomt för att behålla nuvarande värde.[/]");

            string? newGuest = ui.AskWithEsc($"Ny gäst-ID (tomt = {booking.GuestId}):");
            if (newGuest == null) return null;

            string? newRoom = ui.AskWithEsc($"Nytt rum-ID (tomt = {booking.RoomId}):");
            if (newRoom == null) return null;

            string? newFrom = ui.AskWithEsc($"Nytt från-datum (yyyy-MM-dd, tomt = {booking.FromDate:yyyy-MM-dd}):");
            if (newFrom == null) return null;

            string? newTo = ui.AskWithEsc($"Nytt till-datum (yyyy-MM-dd, tomt = {booking.ToDate:yyyy-MM-dd}):");
            if (newTo == null) return null;

            string? newGuests = ui.AskWithEsc($"Nytt antal gäster (tomt = {booking.NumberOfGuests}):");
            if (newGuests == null) return null;

            return new BookingUpdateModel
            {
                GuestId = string.IsNullOrWhiteSpace(newGuest) ? null : int.Parse(newGuest),
                RoomId = string.IsNullOrWhiteSpace(newRoom) ? null : int.Parse(newRoom),
                FromDate = string.IsNullOrWhiteSpace(newFrom) ? null : DateTime.Parse(newFrom),
                ToDate = string.IsNullOrWhiteSpace(newTo) ? null : DateTime.Parse(newTo),
                NumberOfGuests = string.IsNullOrWhiteSpace(newGuests) ? null : int.Parse(newGuests)
            };
        }
    }
}