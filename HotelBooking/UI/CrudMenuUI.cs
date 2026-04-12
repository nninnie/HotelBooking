using HotelBooking.Models;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;

namespace HotelBooking.UI
{
    public static class CrudMenuUI
    {
        public static string Show(string title, IEnumerable<Room>? rooms = null)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader(title);

            if (rooms != null && rooms.Any())
            {
                AnsiConsole.WriteLine();
                ShowRoomTable(rooms);
                AnsiConsole.WriteLine();
            }

            var menu = new SelectionPrompt<string>()
                .Title("[grey]Välj ett alternativ[/]")
                .AddChoices(
                    "Skapa ny",
                    "Visa alla",
                    "Visa detaljer",
                    "Uppdatera",
                    "Ta bort",
                    "Återställ",
                    "Tillbaka"
                );

            // Returnera VALTEXTEN direkt
            return AnsiConsole.Prompt(menu);
        }

        private static void ShowRoomTable(IEnumerable<Room> rooms)
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
    }
}