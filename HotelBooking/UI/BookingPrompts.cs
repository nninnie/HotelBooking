using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using HotelBooking.Core;
using Spectre.Console;

namespace HotelBooking.UI
{
    public static class BookingPrompts
    {
        // Välj kund (ny eller befintlig)
        public static int? SelectOrCreateGuest(EscUI ui, IGuestService guestService)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Välj kundtyp");

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
                    ErrorService.FieldEmpty();
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
                    ErrorService.FieldEmpty();
                    continue;
                }
                break;
            }

            // BirthDate
            string? birthInput = ui.AskWithEsc("Födelsedatum (yyyy-MM-dd):");
            if (birthInput == null) return null;

            if (!DateTime.TryParse(birthInput, out DateTime birthDate))
            {
                ErrorService.InvalidDate();
                ui.WaitForEsc();
                return null;
            }

            // Telefon
            string? phone;
            while (true)
            {
                phone = ui.AskWithEsc("Telefonnummer:");
                if (phone == null) return null;

                if (string.IsNullOrWhiteSpace(phone))
                {
                    ErrorService.FieldEmpty();
                    continue;
                }

                if (phone.Any(c => !char.IsDigit(c)))
                {
                    ErrorService.Error("Telefonnummer får bara innehålla siffror.");
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
                    ErrorService.FieldEmpty();
                    continue;
                }

                if (!email.Contains("@"))
                {
                    ErrorService.Error("Ogiltig email! Måste innehålla @.");
                    continue;
                }
                break;
            }

            var guest = guestService.CreateGuest(firstName, lastName, birthDate, phone, email);

            return guest.Id;
        }

        // Välj befintlig kund
        public static int? SelectExistingGuest(EscUI ui, IGuestService guestService, string subtitle)
        {
            var guests = guestService.GetAllGuests().ToList();

            if (!guests.Any())
            {
                ErrorService.NotFound("Gäster");
                ui.WaitForEsc();
                return null;
            }

            int pageSize = 10;
            int page = 0;
            int index = 0;

            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("Hantera bokningar", subtitle);

                int totalPages = (int)Math.Ceiling(guests.Count / (double)pageSize);

                var pageItems = guests
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (index >= pageItems.Count)
                    index = pageItems.Count - 1;
                if (index < 0)
                    index = 0;

                AnsiConsole.MarkupLine($"[grey]Sida {page + 1} / {totalPages}[/]");
                AnsiConsole.WriteLine();

                for (int i = 0; i < pageItems.Count; i++)
                {
                    var g = pageItems[i];
                    string line = Markup.Escape(
                    $"#{g.Id} | {g.LastName ?? ""} {g.FirstName ?? ""} | {g.Email ?? ""} | {g.PhoneNumber ?? ""} | {g.BirthDate:yyyy-MM-dd}"
                    );




                    if (i == index)
                        AnsiConsole.MarkupLine($"[black on green]> {line}[/]");
                    else
                        AnsiConsole.MarkupLine($"  {line}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index--;
                        if (index < 0)
                        {
                            if (page > 0)
                            {
                                page--;
                                index = pageSize - 1;
                            }
                            else index = 0;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        index++;
                        if (index >= pageItems.Count)
                        {
                            if (page < totalPages - 1)
                            {
                                page++;
                                index = 0;
                            }
                            else index = pageItems.Count - 1;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (page > 0)
                        {
                            page--;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (page < totalPages - 1)
                        {
                            page++;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.Enter:
                        return pageItems[index].Id;

                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }


        // Antal gäster
        public static int? AskGuestCount(EscUI ui)
        {
            AnsiConsole.WriteLine();

            string? input = ui.AskWithEsc("Antal gäster:");
            if (input == null) return null;

            if (!int.TryParse(input, out int count))
            {
                ErrorService.InvalidNumber();
                return null;
            }

            if (count <= 0)
            {
                ErrorService.Error("Antal gäster måste vara minst 1.");
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
                r => Markup.Escape($"{r.RoomNumber} – {r.RoomType} – {r.PricePerNight} kr/natt (Max {r.MaxGuests})")
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
                ErrorService.InvalidNumber();
                return null;
            }

            if (id <= 0)
            {
                ErrorService.Error("ID måste vara ett positivt tal.");
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
            AnsiConsole.MarkupLine("[green]Tryck ENTER för att behålla nuvarande värde.[/]\n");

            string? newGuest = ui.AskWithEsc(
                $"Ny gäst-ID (tomt = {Markup.Escape(booking.GuestId.ToString())}):"
            );
            if (newGuest == null) return null;

            if (!string.IsNullOrWhiteSpace(newGuest) && !int.TryParse(newGuest, out _))
            {
                ErrorService.InvalidNumber();
                return null;
            }

            string? newRoom = ui.AskWithEsc(
                $"Nytt rum-ID (tomt = {Markup.Escape(booking.RoomId.ToString())}):"
            );
            if (newRoom == null) return null;

            if (!string.IsNullOrWhiteSpace(newRoom) && !int.TryParse(newRoom, out _))
            {
                ErrorService.InvalidNumber();
                return null;
            }

            string? newFrom = ui.AskWithEsc(
                $"Nytt från-datum (yyyy-MM-dd, tomt = {Markup.Escape(booking.FromDate.ToString("yyyy-MM-dd"))}):"
            );
            if (newFrom == null) return null;

            if (!string.IsNullOrWhiteSpace(newFrom) && !DateTime.TryParse(newFrom, out _))
            {
                ErrorService.InvalidDate();
                return null;
            }

            string? newTo = ui.AskWithEsc(
                $"Nytt till-datum (yyyy-MM-dd, tomt = {Markup.Escape(booking.ToDate.ToString("yyyy-MM-dd"))}):"
            );
            if (newTo == null) return null;

            if (!string.IsNullOrWhiteSpace(newTo) && !DateTime.TryParse(newTo, out _))
            {
                ErrorService.InvalidDate();
                return null;
            }

            string? newGuests = ui.AskWithEsc(
                $"Nytt antal gäster (tomt = {Markup.Escape(booking.NumberOfGuests.ToString())}):"
            );
            if (newGuests == null) return null;

            if (!string.IsNullOrWhiteSpace(newGuests) && (!int.TryParse(newGuests, out int g) || g <= 0))
            {
                ErrorService.Error("Antal gäster måste vara ett positivt heltal.");
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

        public static bool Confirm(string message)
        {
            while (true)
            {
                var input = AnsiConsole.Prompt(
                    new TextPrompt<string>($"{message} (j/n): ")
                        .AllowEmpty()
                ).Trim().ToLower();

                if (input == "j") return true;
                if (input == "n") return false;

                ErrorService.Error("Ogiltigt val. Skriv j eller n.");
            }
        }

        public static int? SelectBooking(EscUI ui, List<Booking> bookings, string subtitle)
        {
            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades.[/]");
                ui.WaitForEsc();
                return null;
            }

            int pageSize = 10;
            int page = 0;
            int index = 0;

            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("Hantera bokningar", subtitle);

                int totalPages = (int)Math.Ceiling(bookings.Count / (double)pageSize);

                var pageItems = bookings
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (index >= pageItems.Count)
                    index = pageItems.Count - 1;

                AnsiConsole.MarkupLine($"[grey]Sida {page + 1} / {totalPages}[/]");
                AnsiConsole.WriteLine();

                for (int i = 0; i < pageItems.Count; i++)
                {
                    var b = pageItems[i];

                    string guest = b.Guest != null
                        ? $"{b.Guest.FirstName} {b.Guest.LastName}"
                        : "Okänd gäst";

                    string room = b.Room != null
                        ? $"Rum {b.Room.RoomNumber}"
                        : "Okänt rum";

                    string text = $"#{b.Id} – {guest} – {room} – {b.FromDate:yyyy-MM-dd}";

                    if (i == index)
                        AnsiConsole.MarkupLine($"[black on green]> {text}[/]");
                    else
                        AnsiConsole.MarkupLine($"  {text}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index--;
                        if (index < 0)
                        {
                            if (page > 0)
                            {
                                page--;
                                index = pageSize - 1;
                            }
                            else index = 0;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        index++;
                        if (index >= pageItems.Count)
                        {
                            if (page < totalPages - 1)
                            {
                                page++;
                                index = 0;
                            }
                            else index = pageItems.Count - 1;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (page > 0)
                        {
                            page--;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (page < totalPages - 1)
                        {
                            page++;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.Enter:
                        return pageItems[index].Id;

                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }

        public static int? SelectCheckedInBooking(EscUI ui, IBookingService bookingService, string subtitle)
        {
            var bookings = bookingService.GetAll()
                .Where(b => b.IsCheckedIn)
                .ToList();

            return SelectBooking(ui, bookings, subtitle);
        }

        public static int? SelectBookingFromList(EscUI ui, List<Booking> bookings, string subtitle)
        {
            if (bookings == null || bookings.Count == 0)
                return null;

            int pageSize = 10;
            int page = 0;
            int index = 0;

            while (true)
            {
                AnsiConsole.Clear();
                BannerUI.ShowLogo();
                BannerUI.PrintHeader("Hantera bokningar", subtitle);

                int totalPages = (int)Math.Ceiling(bookings.Count / (double)pageSize);

                var pageItems = bookings
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();

                if (index >= pageItems.Count)
                    index = pageItems.Count - 1;
                if (index < 0)
                    index = 0;

                AnsiConsole.MarkupLine($"[grey]Sida {page + 1} / {totalPages}[/]");
                AnsiConsole.WriteLine();

                for (int i = 0; i < pageItems.Count; i++)
                {
                    var b = pageItems[i];

                    // Null-säkra gästdata
                    var g = b.Guest;
                    string first = Markup.Escape(g?.FirstName ?? "");
                    string last = Markup.Escape(g?.LastName ?? "");

                    // Rum
                    string room = b.Room != null
                        ? $"Rum {Markup.Escape(b.Room.RoomNumber.ToString())}"
                        : "Okänt rum";

                    // Text för listan
                    string text =
                        $"#{b.Id} | {first} {last} | {room} | {b.NumberOfGuests} gäster | {b.FromDate:yyyy-MM-dd} → {b.ToDate:yyyy-MM-dd}";


                    if (i == index)
                        AnsiConsole.MarkupLine($"[black on green]> {text}[/]");
                    else
                        AnsiConsole.MarkupLine($"  {text}");
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index--;
                        if (index < 0)
                        {
                            if (page > 0)
                            {
                                page--;
                                index = pageSize - 1;
                            }
                            else index = 0;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        index++;
                        if (index >= pageItems.Count)
                        {
                            if (page < totalPages - 1)
                            {
                                page++;
                                index = 0;
                            }
                            else index = pageItems.Count - 1;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (page > 0)
                        {
                            page--;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (page < totalPages - 1)
                        {
                            page++;
                            index = 0;
                        }
                        break;

                    case ConsoleKey.Enter:
                        return pageItems[index].Id;

                    case ConsoleKey.Escape:
                        return null;
                }
            }
        }


    }
}
