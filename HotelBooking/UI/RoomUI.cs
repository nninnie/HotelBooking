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

        // RUMSTABELL – ENDA VERSIONEN (alla features)
        private void ShowRoomTable(IEnumerable<Room> rooms)
        {
            var table = new Table().RoundedBorder();
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Nummer[/]");
            table.AddColumn("[yellow]Typ[/]");
            table.AddColumn("[yellow]Extra bäddar[/]");
            table.AddColumn("[yellow]Pris/natt[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var r in rooms)
            {
                table.AddRow(
                    r.Id.ToString(),
                    r.RoomNumber,
                    r.RoomType,
                    r.ExtraBeds.ToString(),
                    $"{r.PricePerNight} kr",
                    r.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]"
                );
            }

            AnsiConsole.Write(table);
            WaitForEsc();
        }

        // VISA ALLA RUM
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

        // VISA DETALJER
        public void ShowDetails(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

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

[white]Havsutsikt:[/] {(room.HasSeaView ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Minibar:[/] {(room.HasMinibar ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Husdjur tillåtet:[/] {(room.IsPetFriendly ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Balkong:[/] {(room.HasBalcony ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Badkar:[/] {(room.HasBathtub ? "[green]Ja[/]" : "[red]Nej[/]")}

[white]Status:[/] {(room.IsOutOfService ? "Ur funktion" : "Tillgänglig")}
")
            {
                Header = new PanelHeader("[green]Rumsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            WaitForEsc();
        }

        // SKAPA RUM
        public void Create(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            string number = AnsiConsole.Ask<string>("Ange [yellow]rumsnummer[/]:");
            string type = AnsiConsole.Ask<string>("Ange [yellow]rumstyp[/]:");
            int capacity = AnsiConsole.Ask<int>("Ange [yellow]grundkapacitet[/]:");
            int extraBeds = AnsiConsole.Ask<int>("Ange [yellow]extrabäddar[/] (0–2):");
            decimal price = AnsiConsole.Ask<decimal>("Ange [yellow]pris per natt[/] (kr):");
            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");

            bool hasSeaView = AnsiConsole.Confirm("Har rummet havsutsikt?");
            bool hasMinibar = AnsiConsole.Confirm("Har rummet minibar?");
            bool isPetFriendly = AnsiConsole.Confirm("Är rummet husdjursvänligt?");
            bool hasBalcony = AnsiConsole.Confirm("Har rummet balkong?");
            bool hasBathtub = AnsiConsole.Confirm("Har rummet badkar?");

            var room = _roomService.CreateRoom(
                number,
                type,
                capacity,
                extraBeds,
                price,
                outOfService,
                hasSeaView,
                hasMinibar,
                isPetFriendly,
                hasBalcony,
                hasBathtub
            );

            AnsiConsole.MarkupLine($"[green]Rum skapat![/] ID: {room.Id}");
            WaitForEsc();
        }

        // UPPDATERA RUM
        public void Update(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att uppdatera:");

            var existing = _roomService.GetRoom(id);

            if (existing == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string number = AnsiConsole.Ask<string>($"Rumsnummer ({existing.RoomNumber}):", existing.RoomNumber);
            string type = AnsiConsole.Ask<string>($"Rumstyp ({existing.RoomType}):", existing.RoomType);
            int capacity = AnsiConsole.Ask<int>($"Grundkapacitet ({existing.Capacity}):", existing.Capacity);
            int extraBeds = AnsiConsole.Ask<int>($"Extrabäddar ({existing.ExtraBeds}):", existing.ExtraBeds);
            decimal price = AnsiConsole.Ask<decimal>($"Pris per natt ({existing.PricePerNight}):", existing.PricePerNight);
            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?", existing.IsOutOfService);

            bool hasSeaView = AnsiConsole.Confirm("Har rummet havsutsikt?", existing.HasSeaView);
            bool hasMinibar = AnsiConsole.Confirm("Har rummet minibar?", existing.HasMinibar);
            bool isPetFriendly = AnsiConsole.Confirm("Är rummet husdjursvänligt?", existing.IsPetFriendly);
            bool hasBalcony = AnsiConsole.Confirm("Har rummet balkong?", existing.HasBalcony);
            bool hasBathtub = AnsiConsole.Confirm("Har rummet badkar?", existing.HasBathtub);

            bool success = _roomService.UpdateRoom(
                id,
                number,
                type,
                capacity,
                extraBeds,
                price,
                outOfService,
                hasSeaView,
                hasMinibar,
                isPetFriendly,
                hasBalcony,
                hasBathtub
            );

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet uppdaterades![/]"
                : "[red]Uppdatering misslyckades.[/]");

            WaitForEsc();
        }

        // SOFT DELETE
        public void SoftDelete(string subtitle)
        {
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att markera som ur funktion:");

            bool success = _roomService.SoftDelete(id);

            AnsiConsole.MarkupLine(success
                ? "[yellow]Rummet är nu markerat som ur funktion.[/]"
                : "[red]Rummet hittades inte.[/]");

            WaitForEsc();
        }

        // ÅTERSTÄLL
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

            int id = AnsiConsole.Ask<int>("Ange [yellow]rums-ID[/] att återställa:");

            bool success = _roomService.Restore(id);

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet är nu återställt![/]"
                : "[red]Rummet hittades inte.[/]");

            WaitForEsc();
        }
    }
}