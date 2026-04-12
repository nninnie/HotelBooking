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

        public void ShowAll(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

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

        public void ShowDetails(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            int id;
            while (true)
            {
                id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/]:");

                if (id <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Rums-ID måste vara ett positivt tal![/]");
                    continue;
                }

                break;
            }

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

        public void Create(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            string number;
            while (true)
            {
                number = AnsiConsole.Ask<string>("Ange [yellow]rumsnummer[/]:");

                if (string.IsNullOrWhiteSpace(number))
                {
                    AnsiConsole.MarkupLine("[red]Rumsnummer får inte vara tomt![/]");
                    continue;
                }

                break;
            }

            string type;
            while (true)
            {
                type = AnsiConsole.Ask<string>("Ange [yellow]rumstyp[/] (Enkelrum/Dubbelrum/Svit):");

                if (string.IsNullOrWhiteSpace(type))
                {
                    AnsiConsole.MarkupLine("[red]Rumstyp får inte vara tomt![/]");
                    continue;
                }

                break;
            }

            int capacity;
            while (true)
            {
                capacity = AnsiConsole.Ask<int>("Ange [yellow]grundkapacitet[/]:");

                if (capacity <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Kapacitet måste vara större än 0![/]");
                    continue;
                }

                break;
            }

            int extraBeds;
            while (true)
            {
                extraBeds = AnsiConsole.Ask<int>("Ange [yellow]extrabäddar[/] (0–2):");

                if (extraBeds < 0 || extraBeds > 2)
                {
                    AnsiConsole.MarkupLine("[red]Extrabäddar måste vara mellan 0 och 2![/]");
                    continue;
                }

                break;
            }

            decimal price;
            while (true)
            {
                price = AnsiConsole.Ask<decimal>("Ange [yellow]pris per natt[/] (kr):");

                if (price <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Pris måste vara större än 0![/]");
                    continue;
                }

                break;
            }

            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            var room = _roomService.CreateRoom(number, type, capacity, extraBeds, price, outOfService);

            AnsiConsole.MarkupLine($"[green]Rum skapat![/] ID: {room.Id}");
            WaitForEsc();
        }

        public void Update(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            int id;
            while (true)
            {
                id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att uppdatera:");

                if (id <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Rums-ID måste vara ett positivt tal![/]");
                    continue;
                }

                break;
            }

            var existing = _roomService.GetRoom(id);

            if (existing == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string number = AnsiConsole.Ask<string>($"Rumsnummer ({existing.RoomNumber}):");

            if (!string.IsNullOrWhiteSpace(number) && string.IsNullOrWhiteSpace(number.Trim()))
            {
                AnsiConsole.MarkupLine("[red]Rumsnummer får inte vara tomt![/]");
                WaitForEsc();
                return;
            }

            string type = AnsiConsole.Ask<string>($"Rumstyp ({existing.RoomType}):");

            if (!string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(type.Trim()))
            {
                AnsiConsole.MarkupLine("[red]Rumstyp får inte vara tomt![/]");
                WaitForEsc();
                return;
            }

            int capacity = AnsiConsole.Ask<int>($"Grundkapacitet ({existing.Capacity}):");

            if (capacity <= 0)
            {
                AnsiConsole.MarkupLine("[red]Kapacitet måste vara större än 0![/]");
                WaitForEsc();
                return;
            }

            int extraBeds = AnsiConsole.Ask<int>($"Extrabäddar ({existing.ExtraBeds}):");

            if (extraBeds < 0 || extraBeds > 2)
            {
                AnsiConsole.MarkupLine("[red]Extrabäddar måste vara mellan 0 och 2![/]");
                WaitForEsc();
                return;
            }

            decimal price = AnsiConsole.Ask<decimal>($"Pris per natt ({existing.PricePerNight}):");

            if (price <= 0)
            {
                AnsiConsole.MarkupLine("[red]Pris måste vara större än 0![/]");
                WaitForEsc();
                return;
            }

            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            bool success = _roomService.UpdateRoom(id, number, type, capacity, extraBeds, price, outOfService);

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet uppdaterades![/]"
                : "[red]Uppdatering misslyckades.[/]");

            WaitForEsc();
        }

        public void SoftDelete(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            int id;
            while (true)
            {
                id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att markera som ur funktion:");

                if (id <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Rums-ID måste vara ett positivt tal![/]");
                    continue;
                }

                break;
            }

            bool success = _roomService.SoftDelete(id);

            AnsiConsole.MarkupLine(success
                ? "[yellow]Rummet är nu markerat som ur funktion.[/]"
                : "[red]Rummet hittades inte.[/]");

            WaitForEsc();
        }

        public void Restore(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var deletedRooms = _roomService.GetDeletedRooms();

            if (!deletedRooms.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Det finns inga rum att återställa.[/]");
                WaitForEsc();
                return;
            }

            ShowRoomTable(deletedRooms);

            int id;
            while (true)
            {
                id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att återställa:");

                if (id <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Rums-ID måste vara ett positivt tal![/]");
                    continue;
                }

                break;
            }

            bool success = _roomService.Restore(id);

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet är nu återställt![/]"
                : "[red]Rummet hittades inte.[/]");

            WaitForEsc();
        }
    }
}