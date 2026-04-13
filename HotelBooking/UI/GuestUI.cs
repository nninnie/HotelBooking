using HotelBooking.Models;
using HotelBooking.Services;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class GuestUI : EscUI
    {
        private readonly IGuestService _guestService;
        private readonly IBookingService _bookingService;

        public GuestUI(IGuestService service, IBookingService bookingService)
        {
            _guestService = service;
            _bookingService = bookingService;   
        }

        private void ShowGuestTable(IEnumerable<Guest> guests)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Namn[/]");
            table.AddColumn("[yellow]Telefon[/]");
            table.AddColumn("[yellow]Email[/]");
            table.AddColumn("[yellow]Aktiv bokning[/]");

            var sortedGuests = guests
                .OrderBy(g => g.Id);

            foreach (var g in guests)
            {
                bool hasActiveBooking = g.Bookings?.Any(b => b.IsActive) == true;

                table.AddRow(
                    g.Id.ToString(),
                    $"{g.LastName}, {g.FirstName}",
                    g.PhoneNumber ?? "-",
                    g.Email ?? "-",
                    hasActiveBooking ? "[green]Ja[/]" : "[red]Nej[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        // VISA ALLA GÄSTER
        public void ShowAll(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _guestService.GetAllGuests();
            ShowGuestTable(guests);

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]");
                WaitForEsc();
                return;
            }

            WaitForEsc();
        }

        // VISA DETALJER
        public void ShowDetails(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _guestService.GetAllGuests();
            ShowGuestTable(guests);

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]");
                WaitForEsc();
                return;
            }

            int? id = AskIntOrEsc("Ange gäst-ID att visa");
            if (id == null) return;

            var guest = _guestService
                .GetGuest(id.Value);
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            // Hämta bokningar via service
            var bookings = _bookingService
                .GetBookingsByGuestId(guest.Id);

            int active = bookings
                .Count(b => b.IsActive);
            int history = bookings
                .Count(b => !b.IsActive);
            decimal totalSpent = bookings
                .Sum(b => b.TotalPrice);

            var lastBooking = bookings
                .OrderByDescending(b => b.FromDate)
                .FirstOrDefault();

            string lastBookingText = lastBooking == null
                ? "[grey]Ingen bokningshistorik[/]"
                : $"Rum {lastBooking.Room?.RoomNumber ?? "-"} ({lastBooking.FromDate:MM-dd} – {lastBooking.ToDate:MM-dd})";

            var panel = new Panel($@"
[white]Gäst-ID:[/] {guest.Id}

[white]Förnamn:[/] {guest.FirstName}
[white]Efternamn:[/] {guest.LastName}

[white]Telefon:[/] {(string.IsNullOrWhiteSpace(guest.PhoneNumber) ? "[red]Saknas[/]" : guest.PhoneNumber)}
[white]Email:[/] {(string.IsNullOrWhiteSpace(guest.Email) ? "[red]Saknas[/]" : guest.Email)}

[white]Antal bokningar:[/] {bookings.Count}
[white]Aktiva bokningar:[/] {active}
[white]Historiska bokningar:[/] {history}

[white]Totalt spenderat:[/] {totalSpent} kr

[white]Senaste bokning:[/]
{lastBookingText}
")
            {
                Header = new PanelHeader("[green]Gästinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            WaitForEsc();
        }

        // SKAPA GÄST
        public void Create(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _guestService.GetAllGuests();
            ShowGuestTable(guests);

            string? firstName;
            while (true)
            {
                firstName = AskWithEsc("Ange förnamn:");
                if (firstName == null) return; 
                if (!string.IsNullOrWhiteSpace(firstName)) break;
                AnsiConsole.MarkupLine("[red]Förnamn får inte vara tomt![/]");
            }

            string? lastName;
            while (true)
            {
                lastName = AskWithEsc("Ange efternamn:");
                if (lastName == null) return; 
                if (!string.IsNullOrWhiteSpace(lastName)) break;
                AnsiConsole.MarkupLine("[red]Efternamn får inte vara tomt![/]");
            }

            string? phone;
            while (true)
            {
                phone = AskWithEsc("Telefonnummer (valfritt):");
                if (phone == null) return; 
                if (string.IsNullOrWhiteSpace(phone) || phone.All(char.IsDigit)) break;
                AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
            }

            string? email;
            while (true)
            {
                email = AskWithEsc("Email (valfritt):");
                if (email == null) return; 
                if (string.IsNullOrWhiteSpace(email) || email.Contains("@")) break;
                AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
            }

            var guest = _guestService.CreateGuest(firstName, lastName, phone, email);

            AnsiConsole.MarkupLine($"[green]Gäst skapad![/] ID: {guest.Id}");
            WaitForEsc();
        }

        // UPPDATERA GÄST
        public void Update(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _guestService.GetAllGuests();
            ShowGuestTable(guests);

            int? id = AskIntOrEsc("Ange gäst-ID att uppdatera");
            if (id == null) return;

            var guest = _guestService.GetGuest(id.Value);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string newFirst = AnsiConsole.Ask<string>($"Nytt förnamn (tomt = behåll) [{guest.FirstName}]:");
            string newLast = AnsiConsole.Ask<string>($"Nytt efternamn (tomt = behåll) [{guest.LastName}]:");

            string phone = AnsiConsole.Ask<string>($"Nytt telefonnummer (tomt = behåll) [{guest.PhoneNumber ?? "-"}]:");
            if (!string.IsNullOrWhiteSpace(phone) && phone.Any(c => !char.IsDigit(c)))
            {
                AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
                WaitForEsc();
                return;
            }

            string email = AnsiConsole.Ask<string>($"Ny email (tomt = behåll) [{guest.Email ?? "-"}]:");
            if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            {
                AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                WaitForEsc();
                return;
            }

            bool success = _guestService.UpdateGuest(
                id.Value,
                string.IsNullOrWhiteSpace(newFirst) ? null : newFirst,
                string.IsNullOrWhiteSpace(newLast) ? null : newLast,
                string.IsNullOrWhiteSpace(phone) ? null : phone,
                string.IsNullOrWhiteSpace(email) ? null : email
            );

            AnsiConsole.MarkupLine(success
                ? "[green]Gäst uppdaterad![/]"
                : "[red]Misslyckades med att uppdatera gäst.[/]");

            WaitForEsc();
        }

        // TA BORT GÄST
        public void SoftDelete(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _guestService.GetAllGuests();
            ShowGuestTable(guests);

            int? id = AskIntOrEsc("Ange gäst-ID att ta bort");
            if (id == null) return;

            var guest = _guestService.GetGuest(id.Value);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gästen hittades inte.[/]");
                WaitForEsc();
                return;
            }

            if (_guestService.HasBookings(id.Value))
            {
                AnsiConsole.MarkupLine("[red]Gästen kan inte tas bort eftersom det finns bokningar kopplade till den.[/]");
                WaitForEsc();
                return;
            }

            bool success = _guestService.DeleteGuest(id.Value);

            AnsiConsole.MarkupLine(success
                ? "[green]Gäst borttagen![/]"
                : "[red]Misslyckades med att ta bort gäst.[/]");

            WaitForEsc();
        }
    }
}