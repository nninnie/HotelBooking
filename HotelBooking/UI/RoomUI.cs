using HotelBooking.Services;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class RoomUI
    {
        private readonly IRoomService _roomService;

        public RoomUI(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // Visa alla rum
        public void ShowAll()
        {
            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga rum hittades.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table().RoundedBorder();
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Nummer[/]");
            table.AddColumn("[yellow]Typ[/]");
            table.AddColumn("[yellow]Kapacitet[/]");
            table.AddColumn("[yellow]Extra bäddar[/]");
            table.AddColumn("[yellow]Max gäster[/]");
            table.AddColumn("[yellow]Pris/natt[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var r in rooms)
            {
                table.AddRow(
                    r.Id.ToString(),
                    r.RoomNumber,
                    r.RoomType,
                    r.BaseGuests.ToString(),
                    r.ExtraBeds.ToString(),
                    r.MaxGuests.ToString(),
                    $"{r.PricePerNight} kr",
                    r.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]"
                );
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        // Visa detaljer för ett rum
        public void ShowDetails()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/]:");

            var room = _roomService.GetRoom(id);

            if (room == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                Console.ReadKey();
                return;
            }

            var panel = new Panel($@"
[white]Rums-ID:[/] {room.Id}
[white]Nummer:[/] {room.RoomNumber}
[white]Typ:[/] {room.RoomType}
[white]Grundkapacitet:[/] {room.BaseGuests}
[white]Extrabäddar:[/] {room.ExtraBeds}
[white]Max gäster:[/] {room.MaxGuests}
[white]Pris per natt:[/] {room.PricePerNight} kr
[white]Status:[/] {(room.IsOutOfService ? "Ur funktion" : "Tillgänglig")}
")
            {
                Header = new PanelHeader("[green]Rumsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            Console.ReadKey();
        }

        // Skapa nytt rum
        public void Create()
        {
            string number = AnsiConsole.Ask<string>("Ange [yellow]rumsnummer[/]:");
            string type = AnsiConsole.Ask<string>("Ange [yellow]rumstyp[/] (Single/Double/Suite):");
            int baseGuests = AnsiConsole.Ask<int>("Ange [yellow]grundkapacitet[/]:");
            int extraBeds = AnsiConsole.Ask<int>("Ange [yellow]extrabäddar[/] (0–2):");
            decimal price = AnsiConsole.Ask<decimal>("Ange [yellow]pris per natt[/] (kr):");
            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            var room = _roomService.CreateRoom(number, type, baseGuests, extraBeds, price, outOfService);

            AnsiConsole.MarkupLine($"[green]Rum skapat![/] ID: {room.Id}");
            Console.ReadKey();
        }

        // Uppdatera rum
        public void Update()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att uppdatera:");

            var existing = _roomService.GetRoom(id);
            if (existing == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                Console.ReadKey();
                return;
            }

            string number = AnsiConsole.Ask<string>($"Rumsnummer ({existing.RoomNumber}):");
            string type = AnsiConsole.Ask<string>($"Rumstyp ({existing.RoomType}):");
            int baseGuests = AnsiConsole.Ask<int>($"Grundkapacitet ({existing.BaseGuests}):");
            int extraBeds = AnsiConsole.Ask<int>($"Extrabäddar ({existing.ExtraBeds}):");
            decimal price = AnsiConsole.Ask<decimal>($"Pris per natt ({existing.PricePerNight}):");
            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            bool success = _roomService.UpdateRoom(id, number, type, baseGuests, extraBeds, price, outOfService);

            if (success)
                AnsiConsole.MarkupLine("[green]Rummet uppdaterades![/]");
            else
                AnsiConsole.MarkupLine("[red]Uppdatering misslyckades.[/]");

            Console.ReadKey();
        }

        // Soft delete
        public void SoftDelete()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att markera som ur funktion:");

            bool success = _roomService.SoftDelete(id);

            if (success)
                AnsiConsole.MarkupLine("[yellow]Rummet är nu markerat som ur funktion.[/]");
            else
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");

            Console.ReadKey();
        }

        // Restore
        public void Restore()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att återställa:");

            bool success = _roomService.Restore(id);

            if (success)
                AnsiConsole.MarkupLine("[green]Rummet är nu återställt![/]");
            else
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");

            Console.ReadKey();
        }
    }
}