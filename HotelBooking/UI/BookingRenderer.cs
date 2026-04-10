using HotelBooking.Models;
using Spectre.Console;

namespace HotelBooking.UI
{
    public static class BookingRenderer
    {
        public static void ShowBookingTable(
            IEnumerable<Booking> bookings,
            Func<int, Guest?> getGuest,
            Func<int, Room?> getRoom)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Gäst[/]");
            table.AddColumn("[yellow]Kontakt[/]");
            table.AddColumn("[yellow]Rum[/]");
            table.AddColumn("[yellow]Från[/]");
            table.AddColumn("[yellow]Till[/]");
            table.AddColumn("[yellow]Gäster[/]");
            table.AddColumn("[yellow]Bokad[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var b in bookings)
            {
                var guest = getGuest(b.GuestId);
                var room = getRoom(b.RoomId);

                table.AddRow(
                    b.Id.ToString(),
                    guest?.FullName ?? "Okänd",
                    $"{guest?.Email ?? "-"} / {guest?.PhoneNumber ?? "-"}",
                    room != null ? $"{room.RoomNumber} ({room.RoomType})" : b.RoomId.ToString(),
                    b.FromDate.ToString("yyyy-MM-dd"),
                    b.ToDate.ToString("yyyy-MM-dd"),
                    b.NumberOfGuests.ToString(),
                    b.BookingDate.ToString("yyyy-MM-dd"),
                    b.IsDeleted ? "[red]Raderad[/]" : "[green]Aktiv[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        // Visa detaljer för en bokning
        public static void ShowBookingDetails(Booking booking, Guest? guest, Room? room)
        {
            var panel = new Panel($@"
[white]ID:[/] {booking.Id}

[white]Gäst:[/] {guest?.FullName ?? "Okänd"}
[white]Kontakt:[/] {guest?.Email ?? "-"} / {guest?.PhoneNumber ?? "-"}

[white]Rum:[/] {(room != null ? $"{room.RoomNumber} ({room.RoomType})" : booking.RoomId)}

[white]Från:[/] {booking.FromDate:yyyy-MM-dd}
[white]Till:[/] {booking.ToDate:yyyy-MM-dd}
[white]Antal gäster:[/] {booking.NumberOfGuests}

[white]Bokad:[/] {booking.BookingDate:yyyy-MM-dd}
[white]Status:[/] {(booking.IsDeleted ? "[red]Raderad[/]" : "[green]Aktiv[/]")}
")
            {
                Header = new PanelHeader("[green]Bokningsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
        }

        // Visa felmeddelande
        public static void ShowError(string message)
        {
            AnsiConsole.MarkupLine($"[red]{message}[/]");
        }

        // Visa lyckat meddelande
        public static void ShowSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]{message}[/]");
        }

        // Visa resultat (true/false)
        public static void ShowResult(bool success, string successMessage, string failMessage)
        {
            if (success)
                ShowSuccess(successMessage);
            else
                ShowError(failMessage);
        }
    }
}