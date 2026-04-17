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

            foreach (var g in guests.OrderBy(g => g.Id))
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

            int? id = AskIntOrEsc("Ange gäst-ID att visa:");
            if (id == null) return;

            var guest = _guestService.GetGuest(id.Value);
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var bookings = _bookingService.GetBookingsByGuestId(guest.Id);

            int active = bookings.Count(b => b.IsActive);
            int history = bookings.Count(b => !b.IsActive);
            decimal totalSpent = bookings.Sum(b => b.TotalPrice);

            var lastBooking = bookings.OrderByDescending(b => b.FromDate).FirstOrDefault();

            string lastBookingText = lastBooking == null
    ? "[grey]Ingen bokningshistorik[/]"
    : $"Rum {lastBooking.Room?.RoomNumber ?? "-"} ({lastBooking.FromDate:yyyy-MM-dd} → {lastBooking.ToDate:yyyy-MM-dd})";

            string lastBookingStatus = lastBooking == null
                ? "[grey]-[/]"
                : lastBooking.IsCheckedIn
                    ? "[green]Incheckad[/]"
                    : lastBooking.IsCompleted
                        ? "[grey]Utcheckad[/]"
                        : lastBooking.IsActive
                            ? "[yellow]Kommande[/]"
                            : "[red]Okänd[/]";

            var panel = new Panel($@"
[bold dodgerblue1]Gäst[/]
[grey]ID:[/] [white]{guest.Id}[/]
[grey]Namn:[/] [white]{guest.FirstName} {guest.LastName}[/]
[grey]Telefon:[/] [white]{(string.IsNullOrWhiteSpace(guest.PhoneNumber) ? "[red]Saknas[/]" : guest.PhoneNumber)}[/]
[grey]Email:[/] [white]{(string.IsNullOrWhiteSpace(guest.Email) ? "[red]Saknas[/]" : guest.Email)}[/]

[bold dodgerblue1]Bokningar[/]
[grey]Totalt:[/] [white]{bookings.Count}[/]
[grey]Aktiva:[/] [white]{active}[/]
[grey]Historik:[/] [white]{history}[/]
[grey]Har aktiv bokning:[/] {(active > 0 ? "[green]Ja[/]" : "[red]Nej[/]")}

[bold dodgerblue1]Ekonomi[/]
[grey]Totalt spenderat:[/] [white]{totalSpent} kr[/]

[bold dodgerblue1]Senaste bokning[/]
{lastBookingText}
[grey]Status:[/] {lastBookingStatus}
")
            {
                Header = new PanelHeader("[green]Gästinformation[/]"),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
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
                firstName = AskWithEsc("Ange förnamn: ");
                if (firstName == null) return;
                if (!string.IsNullOrWhiteSpace(firstName)) break;
                AnsiConsole.MarkupLine("[red]Förnamn får inte vara tomt![/]");
            }

            string? lastName;
            while (true)
            {
                lastName = AskWithEsc("Ange efternamn: ");
                if (lastName == null) return;
                if (!string.IsNullOrWhiteSpace(lastName)) break;
                AnsiConsole.MarkupLine("[red]Efternamn får inte vara tomt![/]");
            }

            string? birthInput = AskWithEsc("Födelsedatum (yyyy-MM-dd): ");
            if (birthInput == null) return;

            if (!DateTime.TryParse(birthInput, out DateTime birthDate))
            {
                AnsiConsole.MarkupLine("[red]Ogiltigt datum! Ange formatet yyyy-MM-dd[/]");
                WaitForEsc();
                return;
            }

            string? phone;
            while (true)
            {
                phone = AskWithEsc("Telefonnummer: ");
                if (phone == null) return;
                if (string.IsNullOrWhiteSpace(phone) || phone.All(char.IsDigit)) break;
                AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
            }

            string? email;
            while (true)
            {
                email = AskWithEsc("Email: ");
                if (email == null) return;
                if (string.IsNullOrWhiteSpace(email) || email.Contains("@")) break;
                AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
            }

            // 
            var guest = _guestService.CreateGuest(firstName, lastName, birthDate, phone, email);

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

            int? id = AskIntOrEsc("Ange gäst-ID att uppdatera:");
            if (id == null) return;

            var guest = _guestService.GetGuest(id.Value);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var bookings = _bookingService.GetBookingsByGuestId(guest.Id);

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            int active = bookings.Count(b => b.IsActive);
            int history = bookings.Count(b => !b.IsActive);
            decimal totalSpent = bookings.Sum(b => b.TotalPrice);

            var lastBooking = bookings
                .OrderByDescending(b => b.FromDate)
                .FirstOrDefault();

            string lastBookingText = lastBooking == null
                ? "[grey]Ingen bokningshistorik[/]"
                : $"Rum {lastBooking.Room?.RoomNumber ?? "-"} ({lastBooking.FromDate:MM-dd} – {lastBooking.ToDate:MM-dd})";

            var panel = new Panel($@"[white]Gäst-ID:[/] {guest.Id}
[white]Förnamn:[/] {guest.FirstName}
[white]Efternamn:[/] {guest.LastName}

[white]Telefon:[/] {(string.IsNullOrWhiteSpace(guest.PhoneNumber) ? "[red]Saknas[/]" : guest.PhoneNumber)}
[white]Email:[/] {(string.IsNullOrWhiteSpace(guest.Email) ? "[red]Saknas[/]" : guest.Email)}

[white]Antal bokningar:[/] {bookings.Count}
[white]Aktiva bokningar:[/] {active}
[white]Historiska bokningar:[/] {history}

[white]Totalt spenderat:[/] {totalSpent} kr

[white]Senaste bokning:[/]
{lastBookingText}")
            {
                Header = new PanelHeader("[green]Gästinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);

            string newFirst = AnsiConsole.Prompt(
                new TextPrompt<string>($"Nytt förnamn (ENTER = behåll [green]{guest.FirstName}[/]):")
                    .AllowEmpty()
            );
            if (string.IsNullOrWhiteSpace(newFirst))
                newFirst = guest.FirstName;

            string newLast = AnsiConsole.Prompt(
                new TextPrompt<string>($"Nytt efternamn (ENTER = behåll [green]{guest.LastName}[/]):")
                    .AllowEmpty()
            );
            if (string.IsNullOrWhiteSpace(newLast))
                newLast = guest.LastName;

            string phone = AnsiConsole.Prompt(
                new TextPrompt<string>($"Nytt telefonnummer (ENTER = behåll [green]{guest.PhoneNumber ?? "-"}[/]):")
                    .AllowEmpty()
            );
            if (string.IsNullOrWhiteSpace(phone))
                phone = guest.PhoneNumber ?? "";

            if (!string.IsNullOrWhiteSpace(phone) && phone.Any(c => !char.IsDigit(c)))
            {
                AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
                WaitForEsc();
                return;
            }

            string email = AnsiConsole.Prompt(
                new TextPrompt<string>($"Ny email (ENTER = behåll [green]{guest.Email ?? "-"}[/]):")
                    .AllowEmpty()
            );
            if (string.IsNullOrWhiteSpace(email))
                email = guest.Email ?? "";

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            {
                AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                WaitForEsc();
                return;
            }

            bool success = _guestService.UpdateGuest(
                id.Value,
                newFirst,
                newLast,
                phone,
                email
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

            int? id = AskIntOrEsc("\nAnge gäst-ID att ta bort:");
            if (id == null) return;

            var guest = _guestService.GetGuest(id.Value);

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill markera denna gäst som borttagen?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Åtgärden avbröts.");
                WaitForEsc();
                return;
            }

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

            bool success = _guestService.SoftDelete(id.Value);

            AnsiConsole.MarkupLine(success
                ? "[green]Gäst borttagen![/]"
                : "[red]Misslyckades med att ta bort gäst.[/]");

            WaitForEsc();
        }

        public void Restore()
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera gäster", "Återställ raderad gäst");
            AnsiConsole.WriteLine();

            var deletedGuests = _guestService.GetDeletedGuests();

            if (!deletedGuests.Any())
            {
                BookingRenderer.ShowError("Det finns inga raderade gäster att återställa.");
                WaitForEsc();
                return;
            }

            var table = new Table().RoundedBorder();
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Namn[/]");
            table.AddColumn("[yellow]Email[/]");
            table.AddColumn("[yellow]Telefon[/]");
            table.AddColumn("[yellow]Totala bokningar[/]");
            table.AddColumn("[yellow]Senaste bokning[/]");

            foreach (var g in deletedGuests)
            {
                int totalBookings = _bookingService.GetBookingsByGuestId(g.Id).Count;
                var lastBooking = _bookingService.GetBookingsByGuestId(g.Id)
                    .OrderByDescending(b => b.FromDate)
                    .FirstOrDefault();

                string lastBookingText = lastBooking == null
                    ? "-"
                    : $"{lastBooking.FromDate:yyyy-MM-dd} → {lastBooking.ToDate:yyyy-MM-dd}";

                table.AddRow(
    g.Id.ToString(),
    $"{g.FirstName ?? "-"} {g.LastName ?? "-"}",
    g.Email ?? "-",
    g.PhoneNumber ?? "-",
    totalBookings.ToString(),
    lastBookingText ?? "-"
);

            }


            AnsiConsole.Write(table);

            var id = BookingPrompts.AskBookingId(this); 
            if (id == null) return;

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill återställa denna gäst?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Återställning avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _guestService.Restore(id.Value);

            BookingRenderer.ShowResult(
                success,
                "Gästen har återställts.",
                "Gästen kunde inte återställas."
            );

            WaitForEsc();
        }
    }
}