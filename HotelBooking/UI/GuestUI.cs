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
                    $"{g.LastName}, {g.FirstName}",
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

        // Skapa en gäst
        public void Create()
        {
            CurrentHeader = "SKAPA GÄST";

            // Förnamn
            string firstName;
            while (true)
            {
                firstName = AnsiConsole.Ask<string>("Ange [yellow]förnamn[/]:");

                if (string.IsNullOrWhiteSpace(firstName))
                {
                    AnsiConsole.MarkupLine("[red]Förnamn får inte vara tomt![/]");
                    continue;
                }

                break;
            }

            // Efternamn
            string lastName;
            while (true)
            {
                lastName = AnsiConsole.Ask<string>("Ange [yellow]efternamn[/]:");

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
                phone = AnsiConsole.Ask<string>("Telefonnummer (valfritt):");

                if (!string.IsNullOrWhiteSpace(phone) && phone.Any(c => !char.IsDigit(c)))
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
                email = AnsiConsole.Ask<string>("Email (valfritt):");

                if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
                {
                    AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                    continue;
                }

                break;
            }

            var guest = _service.CreateGuest(firstName, lastName, phone, email);

            AnsiConsole.MarkupLine($"[green]Gäst skapad![/] ID: {guest.Id}");
            WaitForEsc();
        }

        // Uppdatera en gäst
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

            // Förnamn
            string newFirst = AnsiConsole.Ask<string>($"Nytt förnamn (tomt = behåll) [{guest.FirstName}]:");

            // Efternamn
            string newLast = AnsiConsole.Ask<string>($"Nytt efternamn (tomt = behåll) [{guest.LastName}]:");

            // Telefon
            string phone = AnsiConsole.Ask<string>($"Nytt telefonnummer (tomt = behåll) [{guest.PhoneNumber ?? "-"}]:");

            if (!string.IsNullOrWhiteSpace(phone) && phone.Any(c => !char.IsDigit(c)))
            {
                AnsiConsole.MarkupLine("[red]Telefonnummer får bara innehålla siffror![/]");
                WaitForEsc();
                return;
            }

            // Email
            string email = AnsiConsole.Ask<string>($"Ny email (tomt = behåll) [{guest.Email ?? "-"}]:");

            if (!string.IsNullOrWhiteSpace(email) && !email.Contains("@"))
            {
                AnsiConsole.MarkupLine("[red]Ogiltig email! Måste innehålla @[/]");
                WaitForEsc();
                return;
            }

            bool success = _service.UpdateGuest(
                id,
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

        // Ta bort en gäst
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