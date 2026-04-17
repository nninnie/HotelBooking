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
            table.AddColumn("[yellow]Kapacitet[/]");
            table.AddColumn("[yellow]Extrabäddar[/]");
            table.AddColumn("[yellow]Pris/natt[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var r in rooms)
            {
                table.AddRow(
                    r.Id.ToString(),
                    r.RoomNumber,
                    r.RoomType,
                    $"{r.Capacity}",
                    $"{r.ExtraBeds}",
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

            int? id = AskIntOrEsc("\nAnge rums-ID:");
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
[bold dodgerblue1]Rum[/]
[grey]Rums-ID:[/] [white]{room.Id}[/]
[grey]Nummer:[/] [white]{room.RoomNumber}[/]
[grey]Typ:[/] [white]{room.RoomType}[/]
[grey]Ordinarie platser:[/] [white]{room.Capacity}[/]
[grey]Extrabäddar:[/] [white]{room.ExtraBeds}[/]
[grey]Pris per natt:[/] [white]{room.PricePerNight} kr[/]

[bold dodgerblue1]Faciliteter[/]
[grey]Havsutsikt:[/] {(room.HasSeaView ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Minibar:[/] {(room.HasMinibar ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Husdjur tillåtet:[/] {(room.IsPetFriendly ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Balkong:[/] {(room.HasBalcony ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Badkar:[/] {(room.HasBathtub ? "[green]Ja[/]" : "[red]Nej[/]")}

[bold dodgerblue1]Status[/]
[grey]Tillgänglighet:[/] {(room.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]")}
")
            {
                Header = new PanelHeader("[green]Rumsinformation[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
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

            string? number = AskWithEsc("[grey]Ange rumsnummer[/]: ");
            if (number == null) return;

            string? type = AskWithEsc("[grey]Ange rumstyp[/]: ");
            if (type == null) return;

            int squareMeters;
            while (true)
            {
                string? sqmInput = AskWithEsc("[grey]Antal kvadratmeter[/]: ");
                if (sqmInput == null) return;

                if (!int.TryParse(sqmInput, out squareMeters) || squareMeters <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Kvadratmeter måste vara ett positivt heltal![/]");
                    continue;
                }
                break;
            }

            int? capacity = AskIntOrEsc("[grey]Antal ordinarie gäster[/]: ");
            if (capacity == null) return;

            int? extraBeds = AskIntOrEsc("[grey]Antal extrabäddar (0–2)[/]: ");
            if (extraBeds == null) return;

            decimal? price = AskDecimalOrEsc("[grey]Pris per natt (kr)[/]: ");
            if (price == null) return;

            // Nu använder vi DIN Confirm-metod
            bool outOfService = BookingPrompts.Confirm("[grey]Är rummet ur funktion[/]");
            bool hasSeaView = BookingPrompts.Confirm("[grey]Har rummet havsutsikt[/]");
            bool hasMinibar = BookingPrompts.Confirm("[grey]Har rummet minibar[/]");
            bool isPetFriendly = BookingPrompts.Confirm("[grey]Är rummet husdjursvänligt[/]");
            bool hasBalcony = BookingPrompts.Confirm("[grey]Har rummet balkong[/]");
            bool hasBathtub = BookingPrompts.Confirm("[grey]Har rummet badkar[/]");

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

            int? id = AskIntOrEsc("\nAnge rums-ID att uppdatera: ");
            if (id == null) return;

            var existing = _roomService.GetRoom(id.Value);

            if (existing == null)
            {
                AnsiConsole.MarkupLine("[red]Rummet hittades inte.[/]");
                WaitForEsc();
                return;
            }

            // Visa detaljerad panel innan redigering
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera rum", subtitle);

            var panel = new Panel($@"[grey]Rums-ID:[/] [white]{existing.Id}[/]

[bold dodgerblue1]Rum[/]
[grey]Nummer:[/] [white]{existing.RoomNumber}[/]
[grey]Typ:[/] [white]{existing.RoomType}[/]
[grey]Ordinarie platser:[/] [white]{existing.Capacity}[/]
[grey]Extrabäddar:[/] [white]{existing.ExtraBeds}[/]
[grey]Pris per natt:[/] [white]{existing.PricePerNight} kr[/]

[bold dodgerblue1]Faciliteter[/]
[grey]Havsutsikt:[/] {(existing.HasSeaView ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Minibar:[/] {(existing.HasMinibar ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Husdjur tillåtet:[/] {(existing.IsPetFriendly ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Balkong:[/] {(existing.HasBalcony ? "[green]Ja[/]" : "[red]Nej[/]")}
[grey]Badkar:[/] {(existing.HasBathtub ? "[green]Ja[/]" : "[red]Nej[/]")}

[bold dodgerblue1]Status[/]
[grey]Tillgänglighet:[/] {(existing.IsOutOfService ? "[red]Ur funktion[/]" : "[green]Tillgänglig[/]")}
")
            {
                Header = new PanelHeader("[green]Rumsinformation[/]", Justify.Center),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };

            AnsiConsole.Write(panel);

            // Rumsnummer
            string? number = AskWithEsc(
                $"[grey]Rumsnummer[/] (ENTER = behåll [green]{existing.RoomNumber}[/]):"
            );
            if (number == null) return;
            if (number == "") number = existing.RoomNumber;

            // Rumstyp
            string? type = AskWithEsc(
                $"[grey]Rumstyp[/] (ENTER = behåll [green]{existing.RoomType}[/]):"
            );
            if (type == null) return;
            if (type == "") type = existing.RoomType;

            // Ordinarie gäster
            int? capacity = AskIntOrEsc(
                $"[grey]Antal ordinarie gäster[/] (ENTER = behåll [green]{existing.Capacity}[/]):",
                existing.Capacity
            );
            if (capacity == null) return;

            // Extrabäddar
            int? extraBeds = AskIntOrEsc(
                $"[grey]Extrabäddar[/] (ENTER = behåll [green]{existing.ExtraBeds}[/]):",
                existing.ExtraBeds
            );
            if (extraBeds == null) return;

            // Pris per natt
            decimal? price = AskDecimalOrEsc(
                $"[grey]Pris per natt[/] (ENTER = behåll [green]{existing.PricePerNight}[/]):",
                existing.PricePerNight
            );
            if (price == null) return;

            // Ur funktion
            bool? outOfService = AskBoolOrEsc(
                $"[grey]Är rummet ur funktion?[/] (ENTER = behåll [green]{(existing.IsOutOfService ? "Ja" : "Nej")}[/])",
                existing.IsOutOfService
            );
            if (outOfService == null) return;

            // Havsutsikt
            bool? hasSeaView = AskBoolOrEsc(
                $"[grey]Har rummet havsutsikt?[/] (ENTER = behåll [green]{(existing.HasSeaView ? "Ja" : "Nej")}[/])",
                existing.HasSeaView
            );
            if (hasSeaView == null) return;

            // Minibar
            bool? hasMinibar = AskBoolOrEsc(
                $"[grey]Har rummet minibar?[/] (ENTER = behåll [green]{(existing.HasMinibar ? "Ja" : "Nej")}[/])",
                existing.HasMinibar
            );
            if (hasMinibar == null) return;

            // Husdjursvänligt
            bool? isPetFriendly = AskBoolOrEsc(
                $"[grey]Är rummet husdjursvänligt?[/] (ENTER = behåll [green]{(existing.IsPetFriendly ? "Ja" : "Nej")}[/])",
                existing.IsPetFriendly
            );
            if (isPetFriendly == null) return;

            // Balkong
            bool? hasBalcony = AskBoolOrEsc(
                $"[grey]Har rummet balkong?[/] (ENTER = behåll [green]{(existing.HasBalcony ? "Ja" : "Nej")}[/])",
                existing.HasBalcony
            );
            if (hasBalcony == null) return;

            // Badkar
            bool? hasBathtub = AskBoolOrEsc(
                $"[grey]Har rummet badkar?[/] (ENTER = behåll [green]{(existing.HasBathtub ? "Ja" : "Nej")}[/])",
                existing.HasBathtub
            );
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
