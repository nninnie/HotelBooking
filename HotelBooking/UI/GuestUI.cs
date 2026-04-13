using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class GuestUI : EscUI
    {
        private readonly IGuestService _service;

        public GuestUI(IGuestService service)
        {
            _service = service;
        }

        private void ShowGuestTable(IEnumerable<Guest> guests)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Namn[/]");
            table.AddColumn("[yellow]Telefon[/]");
            table.AddColumn("[yellow]Email[/]");

            foreach (var g in guests)
            {
                table.AddRow(
                    g.Id.ToString(),
                    $"{g.LastName}, {g.FirstName}",
                    g.PhoneNumber ?? "-",
                    g.Email ?? "-"
                );
            }

            AnsiConsole.Write(table);
        }

        // VISA ALLA GÄSTER
        public void ShowAll(string subtitle)
        {
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _service.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowGuestTable(guests);
            WaitForEsc();
        }

        // VISA DETALJER
        public void ShowDetails(string subtitle)
        {
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _service.GetAllGuests();
            ShowGuestTable(guests);

            int? id = AskIntOrEsc("Ange gäst-ID");
            if (id == null) return;

            var guest = _service.GetGuest(id.Value);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var panel = new Panel($@"
[white]ID:[/] {guest.Id}
[white]Namn:[/] {guest.LastName}, {guest.FirstName}
[white]Telefon:[/] {guest.PhoneNumber ?? "-"}
[white]Email:[/] {guest.Email ?? "-"}
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
            BannerUI.PrintHeader("Hantera gäster", subtitle);

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

            var guest = _service.CreateGuest(firstName, lastName, phone, email);

            AnsiConsole.MarkupLine($"[green]Gäst skapad![/] ID: {guest.Id}");
            WaitForEsc();
        }

        // UPPDATERA GÄST
        public void Update(string subtitle)
        {
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _service.GetAllGuests();
            ShowGuestTable(guests);

            int? id = AskIntOrEsc("Ange gäst-ID att uppdatera");
            if (id == null) return;

            var guest = _service.GetGuest(id.Value);

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

            bool success = _service.UpdateGuest(
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
            BannerUI.PrintHeader("Hantera gäster", subtitle);

            var guests = _service.GetAllGuests();
            ShowGuestTable(guests);

            int? id = AskIntOrEsc("Ange gäst-ID att ta bort");
            if (id == null) return;

            var guest = _service.GetGuest(id.Value);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gästen hittades inte.[/]");
                WaitForEsc();
                return;
            }

            if (_service.HasBookings(id.Value))
            {
                AnsiConsole.MarkupLine("[red]Gästen kan inte tas bort eftersom det finns bokningar kopplade till den.[/]");
                WaitForEsc();
                return;
            }

            bool success = _service.DeleteGuest(id.Value);

            AnsiConsole.MarkupLine(success
                ? "[green]Gäst borttagen![/]"
                : "[red]Misslyckades med att ta bort gäst.[/]");

            WaitForEsc();
        }
    }
}