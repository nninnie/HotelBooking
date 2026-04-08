using Spectre.Console;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.UI
{
    public class GuestUI : EscUI
    {
        private readonly IGuestService _service;

        public GuestUI(IGuestService service)
        {
            _service = service;
        }

        public void ShowAll()
        {
            var guests = _service.GetAllGuests();

            if (!guests.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga gäster hittades.[/]");
                WaitForEsc();
                return;
            }

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
            WaitForEsc();
        }

        public void ShowDetails()
        {
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

        public void Create()
        {
            string fullName = AnsiConsole.Ask<string>("Ange [yellow]fullständigt namn[/]:");
            string? phone = AnsiConsole.Ask<string>("Telefonnummer (valfritt):");
            string? email = AnsiConsole.Ask<string>("Email (valfritt):");

            var guest = _service.CreateGuest(fullName, phone, email);

            AnsiConsole.MarkupLine($"[green]Gäst skapad![/] ID: {guest.Id}");
            WaitForEsc();
        }

        public void Update()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]gäst-ID[/]:");

            var guest = _service.GetGuest(id);
            if (guest == null)
            {
                AnsiConsole.MarkupLine("[red]Gäst hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string fullName = AnsiConsole.Ask<string>($"Nytt namn (tomt = behåll) [{guest.FullName}]:");
            string phone = AnsiConsole.Ask<string>($"Nytt telefonnummer (tomt = behåll) [{guest.PhoneNumber ?? "-"}]:");
            string email = AnsiConsole.Ask<string>($"Ny email (tomt = behåll) [{guest.Email ?? "-"}]:");

            bool success = _service.UpdateGuest(
                id,
                string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                string.IsNullOrWhiteSpace(phone) ? null : phone,
                string.IsNullOrWhiteSpace(email) ? null : email
            );

            if (success)
                AnsiConsole.MarkupLine("[green]Gäst uppdaterad![/]");
            else
                AnsiConsole.MarkupLine("[red]Misslyckades med att uppdatera gäst.[/]");

            WaitForEsc();
        }
    }
}