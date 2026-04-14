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

        // RUMSTABELL 
        private void ShowRoomTable(IEnumerable<Room> rooms)
        {
            var table = new Table().RoundedBorder();
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Nummer[/]");
            table.AddColumn("[yellow]Typ[/]");
            table.AddColumn("[yellow]Max antal gäster[/]");
            table.AddColumn("[yellow]Pris/natt[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var r in rooms)
            {
                table.AddRow(
                    r.Id.ToString(),
                    r.RoomNumber,
                    r.RoomType,
                    $"{r.Capacity} + {r.ExtraBeds}",
                    $"{r.PricePerNight} kr",
                    r.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        // VISA ALLA RUM
        public void ShowAll(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
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
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            int? id = AskIntOrEsc("Ange rums-ID:");
            if (id == null) return;

            var room = _roomService.GetRoom(id.Value);

            if (room == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var panel = new Panel($@"
[white]Rums-ID:[/] {room.Id}
[white]Nummer:[/] {room.RoomNumber}
[white]Typ:[/] {room.RoomType}
[white]Ordinarie platser:[/] {room.Capacity}
[white]Extrabäddar:[/] {room.ExtraBeds}
[white]Pris per natt:[/] {room.PricePerNight} kr
[white]Havsutsikt:[/] {(room.HasSeaView ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Minibar:[/] {(room.HasMinibar ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Husdjur tillåtet:[/] {(room.IsPetFriendly ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Balkong:[/] {(room.HasBalcony ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Badkar:[/] {(room.HasBathtub ? "[green]Ja[/]" : "[red]Nej[/]")}
[white]Status:[/] {(room.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]")}
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
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);

            string? number = AskWithEsc("Ange rumsnummer:");
            if (number == null) return;

            string? type = AskWithEsc("Ange rumstyp:");
            if (type == null) return;


            int squareMeters;
            while (true)
            {
                string? sqmInput = AskWithEsc("Antal kvadratmeter:");
                if (sqmInput == null) return;

                if (!int.TryParse(sqmInput, out squareMeters) || squareMeters <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Kvadratmeter måste vara ett positivt heltal![/]");
                    continue;
                }
                break;
            }

            int? capacity = AskIntOrEsc("Ange antal ordinarie gäster:");
            if (capacity == null) return;

            int? extraBeds = AskIntOrEsc("Ange extrabäddar (0–2):");
            if (extraBeds == null) return;

            decimal? price = AskDecimalOrEsc("Ange pris per natt (kr):");
            if (price == null) return;

            bool outOfService = AnsiConsole.Confirm("Är rummet ur funktion?");
            bool hasSeaView = AnsiConsole.Confirm("Har rummet havsutsikt?");
            bool hasMinibar = AnsiConsole.Confirm("Har rummet minibar?");
            bool isPetFriendly = AnsiConsole.Confirm("Är rummet husdjursvänligt?");
            bool hasBalcony = AnsiConsole.Confirm("Har rummet balkong?");
            bool hasBathtub = AnsiConsole.Confirm("Har rummet badkar?");

            var room = _roomService.CreateRoom(
                number, type, squareMeters, capacity.Value, extraBeds.Value, price.Value,
                outOfService, hasSeaView, hasMinibar,
                isPetFriendly, hasBalcony, hasBathtub
                );

            AnsiConsole.MarkupLine($"[green]Rum skapat![/] ID: {room.Id}");
            WaitForEsc();
        }

        // UPPDATERA RUM
        public void Update(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var rooms = _roomService.GetAllRooms();
            ShowRoomTable(rooms);
              
            int? id = AskIntOrEsc("Ange rums-ID att uppdatera");
            if (id == null) return;

            var existing = _roomService.GetRoom(id.Value);

            if (existing == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            string? number = AskWithEsc($"Rumsnummer ({existing.RoomNumber}):");
            if (number == null) return;
            if (number == "") number = existing.RoomNumber;

            string? type = AskWithEsc($"Rumstyp ({existing.RoomType}):");
            if (type == null) return;
            if (type == "") type = existing.RoomType;

            int? capacity = AskIntOrEsc($"Antal ordinarie gäster ({existing.Capacity}):");
            if (capacity == null) return;


            int? extraBeds = AskIntOrEsc($"Extrabäddar ({existing.ExtraBeds}):");
            if (extraBeds == null) return;

            decimal? price = AskDecimalOrEsc($"Pris per natt ({existing.PricePerNight}):");
            if (price == null) return;

            bool? outOfService = AskBoolOrEsc("Är rummet ur funktion?", existing.IsOutOfService);
            if (outOfService == null) return;

            bool? hasSeaView = AskBoolOrEsc("Har rummet havsutsikt?", existing.HasSeaView);
            if (hasSeaView == null) return;

            bool? hasMinibar = AskBoolOrEsc("Har rummet minibar?", existing.HasMinibar);
            if (hasMinibar == null) return;

            bool? isPetFriendly = AskBoolOrEsc("Är rummet husdjursvänligt?", existing.IsPetFriendly);
            if (isPetFriendly == null) return;

            bool? hasBalcony = AskBoolOrEsc("Har rummet balkong?", existing.HasBalcony);
            if (hasBalcony == null) return;

            bool? hasBathtub = AskBoolOrEsc("Har rummet badkar?", existing.HasBathtub);
            if (hasBathtub == null) return;

            bool success = _roomService.UpdateRoom(
                id.Value,
                number,
                type,
                capacity.Value,
                extraBeds.Value,
                price.Value,
                outOfService.Value,
                hasSeaView.Value,
                hasMinibar.Value,
                isPetFriendly.Value,
                hasBalcony.Value,
                hasBathtub.Value
            );

            AnsiConsole.MarkupLine(success
                ? "[green]Rummet uppdaterades![/]"
                : "[red]Uppdatering misslyckades.[/]");

            WaitForEsc();
        }
        // SOFT DELETE (MARKERA SOM UR FUNKTION)    
        public void SoftDelete(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);
            AnsiConsole.WriteLine();

            var rooms = _roomService.GetAllRooms();

            if (!rooms.Any())
            {
                BookingRenderer.ShowError("Inga rum hittades.");
                WaitForEsc();
                return;
            }

            ShowRoomTable(rooms);

            int? id = AskIntOrEsc("Ange rums-ID att markera som ur funktion");
            if (id == null) return;

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill markera detta rum som ur funktion?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Åtgärden avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _roomService.SoftDelete(id.Value);

            BookingRenderer.ShowResult(
                success,
                "Rummet är nu markerat som ur funktion.",
                "Rummet hittades inte."
            );

            WaitForEsc();
        }

        // ÅTERSTÄLL
        public void Restore(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);
            AnsiConsole.WriteLine();

            var deletedRooms = _roomService.GetDeletedRooms();

            if (!deletedRooms.Any())
            {
                BookingRenderer.ShowError("Det finns inga rum att återställa.");
                WaitForEsc();
                return;
            }

            ShowRoomTable(deletedRooms);

            int? id = AskIntOrEsc("Ange rums-ID att återställa");
            if (id == null) return;

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill återställa detta rum?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Återställning avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _roomService.Restore(id.Value);

            BookingRenderer.ShowResult(
                success,
                "Rummet är nu återställt!",
                "Rummet hittades inte."
            );

            WaitForEsc();
        }
    }
}