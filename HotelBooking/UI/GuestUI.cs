using HotelBooking.Models;
using HotelBooking.Services;
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

        // Gemensam metod för att visa gäster i tabellform
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
                    g.FullName,
                    g.PhoneNumber ?? "-",
                    g.Email ?? "-"
                );
            }

            AnsiConsole.Write(table);
        }

        // Visa alla gäster
        public void ShowAll()
        {
            CurrentHeader = "VISA ALLA GÄSTER";

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

        // Visa detaljer på en gäst
        public void ShowDetails()
        {
            CurrentHeader = "GÄSTDETALJER";

            var guests = _service.GetAllGuests();
            ShowGuestTable(guests);

            int id = AnsiConsole.Ask<int>("Ange [yellow]gäst-ID[/]:");
            var guest = _service.GetGuest(id);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var panel = new Panel($@"
[white]ID:[/] {guest.Id}
[white]Namn:[/] {guest.FullName}
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

        // Skapa en gäst
        public void Create()
        {
            CurrentHeader = "SKAPA GÄST";

            string? fullName;

            while (true)
            {
                fullName = AnsiConsole.Ask<string>("Ange [yellow]fullständigt namn[/]:");

                // VALIDATION
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    AnsiConsole.MarkupLine("[red]Namn får inte vara tomt![/]");
                    continue;
                }

                break;
            }

            string? phone;
            while (true)
            {
                phone = AnsiConsole.Ask<string>("Telefonnummer (valfritt):");

                // VALIDATION
                if (!string.IsNullOrWhiteSpace(phone) && phone.Any(c => !char.IsDigit(c)))
                {
                    AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
                    continue;
                }

                break;
            }

            string? email;
            while (true)
            {
                email = AnsiConsole.Ask<string>("Email (valfritt):");

                // VALIDATION
                if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
                {
                    AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                    continue;
                }

                break;
            }

            var guest = _service.CreateGuest(fullName, phone, email);

            AnsiConsole.MarkupLine($"[green]Gäst skapad![/] ID: {guest.Id}");
            WaitForEsc();
        }

        // Updatera en gäst
        public void Update()
        {
            CurrentHeader = "UPPDATERA GÄST";

            var guests = _service.GetAllGuests();
            ShowGuestTable(guests);

            int id = AnsiConsole.Ask<int>("Ange [yellow]gäst-ID[/] att uppdatera:");
            var guest = _service.GetGuest(id);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string fullName = AnsiConsole.Ask<string>($"Nytt namn (tomt = behåll) [{guest.FullName}]:");

            // VALIDATION
            if (!string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(fullName.Trim()))
            {
                AnsiConsole.MarkupLine("[red]Namn får inte vara tomt![/]");
                WaitForEsc();
                return;
            }

            string phone = AnsiConsole.Ask<string>($"Nytt telefonnummer (tomt = behåll) [{guest.PhoneNumber ?? "-"}]:");

            // VALIDATION
            if (!string.IsNullOrWhiteSpace(phone) && phone.Any(c => !char.IsDigit(c)))
            {
                AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
                WaitForEsc();
                return;
            }

            string email = AnsiConsole.Ask<string>($"Ny email (tomt = behåll) [{guest.Email ?? "-"}]:");

            // VALIDATION
            if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            {
                AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                WaitForEsc();
                return;
            }

            bool success = _service.UpdateGuest(
                id,
                string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                string.IsNullOrWhiteSpace(phone) ? null : phone,
                string.IsNullOrWhiteSpace(email) ? null : email
            );

            AnsiConsole.MarkupLine(success
                ? "[green]Gäst uppdaterad![/]"
                : "[red]Misslyckades med att uppdatera gäst.[/]");

            WaitForEsc();
        }

        // Ta bort en gäst (soft delete)
        public void SoftDelete()
        {
            CurrentHeader = "TA BORT GÄST";

            var guests = _service.GetAllGuests();
            ShowGuestTable(guests);

            int id = AnsiConsole.Ask<int>("Ange [yellow]gäst-ID[/] att ta bort:");
            var guest = _service.GetGuest(id);

            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gästen hittades inte.[/]");
                WaitForEsc();
                return;
            }

            // Kontrollera om gästen har bokningar
            if (_service.HasBookings(id))
            {
                AnsiConsole.MarkupLine("[red]Gästen kan inte tas bort eftersom det finns bokningar kopplade till den.[/]");
                WaitForEsc();
                return;
            }

            bool success = _service.DeleteGuest(id);

            AnsiConsole.MarkupLine(success
                ? "[green]Gäst borttagen![/]"
                : "[red]Misslyckades med att ta bort gäst.[/]");

            WaitForEsc();
        }

    }
}