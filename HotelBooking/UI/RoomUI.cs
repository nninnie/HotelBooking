using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class RoomUI : EscUI
    {
        private readonly IRoomService _roomService;

        public RoomUI(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // Visa lista med alla rum i början av all Roomm CRUD, så att användaren ser vilka rum som finns och deras ID:n.
        private void ShowRoomTable(IEnumerable<Room> rooms)
        {
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
                    r.Capacity.ToString(),
                    r.ExtraBeds.ToString(),
                    r.MaxGuests.ToString(),
                    $"{r.PricePerNight} kr",
                    r.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        public void ShowAll()
        {
            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga rum hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowRoomTable(rooms);
            WaitForEsc();
        }

        public void ShowDetails()
        {
            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/]:");
            var room = _roomService.GetRoom(id);

            if (room == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var panel = new Panel($@"
[white]Rums-ID:[/] {room.Id}
[white]Nummer:[/] {room.RoomNumber}
[white]Typ:[/] {room.RoomType}
[white]Grundkapacitet:[/] {room.Capacity}
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
            WaitForEsc();
        }

        public void Create()
        {
            var rooms = _roomService.GetAllRooms();
            AnsiConsole.MarkupLine("[yellow]Befintliga rum:[/]\n");

            if (rooms.Any())
                ShowRoomTable(rooms);
            else
                AnsiConsole.MarkupLine("[grey]Inga rum finns ännu.[/]");

            AnsiConsole.WriteLine();

            string number = AnsiConsole.Ask<string>("Ange [yellow]rumsnummer[/]:");
            string type = AnsiConsole.Ask<string>("Ange [yellow]rumstyp[/] (Single/Double/Suite):");
            int capacity = AnsiConsole.Ask<int>("Ange [yellow]grundkapacitet[/]:");
            int extraBeds = AnsiConsole.Ask<int>("Ange [yellow]extrabäddar[/] (0–2):");
            decimal price = AnsiConsole.Ask<decimal>("Ange [yellow]pris per natt[/] (kr):");
            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            var room = _roomService.CreateRoom(number, type, capacity, extraBeds, price, outOfService);

            AnsiConsole.MarkupLine($"[green]Rum skapat![/] ID: {room.Id}");
            WaitForEsc();
        }

        public void Update()
        {
            var rooms = _roomService.GetAllRooms();
            AnsiConsole.MarkupLine("[yellow]Befintliga rum:[/]\n");

            if (rooms.Any())
                ShowRoomTable(rooms);
            else
            {
                AnsiConsole.MarkupLine("[grey]Inga rum finns ännu.[/]");
                WaitForEsc();
                return;
            }

            AnsiConsole.WriteLine();

            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att uppdatera:");
            var existing = _roomService.GetRoom(id);

            if (existing == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string number = AnsiConsole.Ask<string>($"Rumsnummer ({existing.RoomNumber}):");
            string type = AnsiConsole.Ask<string>($"Rumstyp ({existing.RoomType}):");
            int capacity = AnsiConsole.Ask<int>($"Grundkapacitet ({existing.Capacity}):");
            int extraBeds = AnsiConsole.Ask<int>($"Extrabäddar ({existing.ExtraBeds}):");
            decimal price = AnsiConsole.Ask<decimal>($"Pris per natt ({existing.PricePerNight}):");
            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            bool success = _roomService.UpdateRoom(id, number, type, capacity, extraBeds, price, outOfService);

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet uppdaterades![/]"
                : "[red]Uppdatering misslyckades.[/]");

            WaitForEsc();
        }

        public void SoftDelete()
        {
            var rooms = _roomService.GetAllRooms();
            AnsiConsole.MarkupLine("[yellow]Befintliga rum:[/]\n");

            if (rooms.Any())
                ShowRoomTable(rooms);
            else
            {
                AnsiConsole.MarkupLine("[grey]Inga rum finns ännu.[/]");
                WaitForEsc();
                return;
            }

            AnsiConsole.WriteLine();

            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att markera som ur funktion:");
            bool success = _roomService.SoftDelete(id);

            AnsiConsole.MarkupLine(success
                ? "[yellow]Rummet är nu markerat som ur funktion.[/]"
                : "[red]Rummet hittades inte.[/]");

            WaitForEsc();
        }

        public void Restore()
        {
            var rooms = _roomService.GetAllRooms();
            AnsiConsole.MarkupLine("[yellow]Befintliga rum:[/]\n");

            if (rooms.Any())
                ShowRoomTable(rooms);
            else
            {
                AnsiConsole.MarkupLine("[grey]Inga rum finns ännu.[/]");
                WaitForEsc();
                return;
            }

            AnsiConsole.WriteLine();

            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att återställa:");
            bool success = _roomService.Restore(id);

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet är nu återställt![/]"
                : "[red]Rummet hittades inte.[/]");

            WaitForEsc();
        }
    }
}