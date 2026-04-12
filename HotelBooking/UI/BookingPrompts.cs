using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public static class BookingPrompts
    {
        // Välj kund (ny eller befintlig)
        public static int? SelectOrCreateGuest(EscUI ui, IGuestService guestService)
        {
            AnsiConsole.Clear();

            // RITA HEADER MED SPECTRE 
            var panel = new Panel("[bold white]HANTERA BOKNINGAR[/]\n[grey]Välj kundtyp[/]");
            panel.Border = BoxBorder.Rounded;
            panel.Expand = false;
            AnsiConsole.Write(panel);

            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

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
                return CreateGuest(ui, guestService, "Ny kund");

            return SelectExistingGuest(ui, guestService, "Välj kund");
        }

        // Skapa ny kund
        public static int? CreateGuest(EscUI ui, IGuestService guestService, string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Ange information för den nya kunden.[/]");
            AnsiConsole.MarkupLine("[grey]Tryck ESC för att avbryta.[/]");

            // Förnamn
            string? firstName;
            while (true)
            {
                firstName = ui.AskWithEsc("\nFörnamn:");
                if (firstName == null) return null;

                if (string.IsNullOrWhiteSpace(firstName))
                {
                    AnsiConsole.MarkupLine("[red]Förnamn får inte vara tomt![/]");
                    continue;
                }
                break;
            }

            // Efternamn
            string? lastName;
            while (true)
            {
                lastName = ui.AskWithEsc("Efternamn:");
                if (lastName == null) return null;

                if (string.IsNullOrWhiteSpace(lastName))
                {
                    AnsiConsole.MarkupLine("[red]Efternamn får inte vara tomt![/]");
                    continue;
                }
                break;
            }

            // Telefon
            string? phone;
            while (true)
            {
                phone = ui.AskWithEsc("Telefonnummer:");
                if (phone == null) return null;

                if (string.IsNullOrWhiteSpace(phone))
                {
                    AnsiConsole.MarkupLine("[red]Telefonnummer är obligatoriskt![/]");
                    continue;
                }

                if (phone.Any(c => !char.IsDigit(c)))
                {
                    AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
                    continue;
                }
                break;
            }

            // Email
            string? email;
            while (true)
            {
                email = ui.AskWithEsc("Email:");
                if (email == null) return null;

                if (string.IsNullOrWhiteSpace(email))
                {
                    AnsiConsole.MarkupLine("[red]Email är obligatoriskt![/]");
                    continue;
                }

                if (!email.Contains("@"))
                {
                    AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                    continue;
                }
                break;
            }

            var guest = guestService.CreateGuest(firstName, lastName, phone, email);
            return guest.Id;
        }

        // Välj befintlig kund
        public static int? SelectExistingGuest(EscUI ui, IGuestService guestService, string subtitle)
        {

            AnsiConsole.WriteLine();

            var guests = guestService.GetAllGuests();

            if (!guests.Any())
                return null;

            var selected = ui.SelectWithEsc(
                guests.ToList(),
                g => $"{g.LastName}, {g.FirstName} – {g.Email} – {g.PhoneNumber}"
            );

            return selected?.Id;
        }

        // Antal gäster
        public static int? AskGuestCount(EscUI ui)
        {
            AnsiConsole.WriteLine();

            string? input = ui.AskWithEsc("Antal gäster:");
            if (input == null) return null;

            if (!int.TryParse(input, out int count))
            {
                AnsiConsole.MarkupLine("[red]Du måste ange ett heltal![/]");
                return null;
            }

            if (count <= 0)
            {
                AnsiConsole.MarkupLine("[red]Antal gäster måste vara minst 1![/]");
                return null;
            }

            return count;
        }

        // Välj rum
        public static int? SelectRoom(EscUI ui, IEnumerable<Room> rooms)
        {
            AnsiConsole.WriteLine();

            var selected = ui.SelectWithEsc(
                rooms.ToList(),
                r => $"{r.RoomNumber} – {r.RoomType} – {r.PricePerNight} kr/natt (Max {r.MaxGuests})"
            );

            return selected?.Id;
        }

        // Fråga efter boknings-ID
        public static int? AskBookingId(EscUI ui)
        {
            AnsiConsole.WriteLine();

            string? input = ui.AskWithEsc("Ange boknings-ID:");
            if (input == null) return null;

            if (!int.TryParse(input, out int id))
            {
                AnsiConsole.MarkupLine("[red]Du måste ange ett heltal![/]");
                return null;
            }

            if (id <= 0)
            {
                AnsiConsole.MarkupLine("[red]ID måste vara ett positivt tal![/]");
                return null;
            }

            return id;
        }

        // Modell för uppdatering av bokning
        public class BookingUpdateModel
        {
            public int? GuestId { get; set; }
            public int? RoomId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public int? NumberOfGuests { get; set; }
        }

        // Fråga efter uppdateringar
        public static BookingUpdateModel? AskBookingUpdates(EscUI ui, Booking booking)
        {
            BannerUI.PrintHeader("Hantera bokningar", "Uppdatera bokning");
            AnsiConsole.WriteLine();
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("[green]Lämna tomt för att behålla nuvarande värde.[/]");

            string? newGuest = ui.AskWithEsc($"Ny gäst-ID (tomt = {booking.GuestId}):");
            if (newGuest == null) return null;

            if (!string.IsNullOrWhiteSpace(newGuest) && !int.TryParse(newGuest, out _))
            {
                AnsiConsole.MarkupLine("[red]Gäst-ID måste vara ett heltal![/]");
                return null;
            }

            string? newRoom = ui.AskWithEsc($"Nytt rum-ID (tomt = {booking.RoomId}):");
            if (newRoom == null) return null;

            if (!string.IsNullOrWhiteSpace(newRoom) && !int.TryParse(newRoom, out _))
            {
                AnsiConsole.MarkupLine("[red]Rum-ID måste vara ett heltal![/]");
                return null;
            }

            string? newFrom = ui.AskWithEsc($"Nytt från-datum (yyyy-MM-dd, tomt = {booking.FromDate:yyyy-MM-dd}):");
            if (newFrom == null) return null;

            if (!string.IsNullOrWhiteSpace(newFrom) && !DateTime.TryParse(newFrom, out _))
            {
                AnsiConsole.MarkupLine("[red]Ogiltigt datumformat![/]");
                return null;
            }

            string? newTo = ui.AskWithEsc($"Nytt till-datum (yyyy-MM-dd, tomt = {booking.ToDate:yyyy-MM-dd}):");
            if (newTo == null) return null;

            if (!string.IsNullOrWhiteSpace(newTo) && !DateTime.TryParse(newTo, out _))
            {
                AnsiConsole.MarkupLine("[red]Ogiltigt datumformat![/]");
                return null;
            }

            string? newGuests = ui.AskWithEsc($"Nytt antal gäster (tomt = {booking.NumberOfGuests}):");
            if (newGuests == null) return null;

            if (!string.IsNullOrWhiteSpace(newGuests) && (!int.TryParse(newGuests, out int g) || g <= 0))
            {
                AnsiConsole.MarkupLine("[red]Antal gäster måste vara ett positivt heltal![/]");
                return null;
            }

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