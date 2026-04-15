using HotelBooking.Models;
using HotelBooking.Core;
using Spectre.Console;

namespace HotelBooking.UI
{
    public static class BookingRenderer
    {
        public static void ShowBookingDetails(Booking booking, Guest? guest, Room? room)
        {
            // Prisberäkning
            int nights = (booking.ToDate.Date - booking.FromDate.Date).Days;
            if (nights < 1) nights = 1;

            decimal pricePerNight = room?.PricePerNight ?? 0;
            decimal totalPrice = nights * pricePerNight;

            // Betald-status
            string paid = booking.Invoice?.IsPaid == true
                ? "[green]✔[/]"
                : "[red]✘[/]";

            // Rumsegenskaper
            string features = room == null
                ? "-"
                : $@"Kapacitet: {room.Capacity}
Extrabäddar: {room.ExtraBeds}
Havsutsikt: {(room.HasSeaView ? "Ja" : "Nej")}
Minibar: {(room.HasMinibar ? "Ja" : "Nej")}
Husdjur tillåtet: {(room.IsPetFriendly ? "Ja" : "Nej")}
Balkong: {(room.HasBalcony ? "Ja" : "Nej")}
Badkar: {(room.HasBathtub ? "Ja" : "Nej")}";

            var panel = new Panel($@"
[white]ID:[/] {booking.Id}

[white]Gäst:[/] {(guest != null ? $"{guest.LastName}, {guest.FirstName}" : "Okänd")}
[white]Kontakt:[/] {guest?.Email ?? "-"} / {guest?.PhoneNumber ?? "-"}

[white]Rum:[/] {(room != null ? $"{room.RoomNumber} ({room.RoomType})" : booking.RoomId)}

[white]Från:[/] {booking.FromDate:yyyy-MM-dd}
[white]Till:[/] {booking.ToDate:yyyy-MM-dd}
[white]Antal nätter:[/] {nights}
[white]Antal gäster:[/] {booking.NumberOfGuests}

[white]Pris per natt:[/] {pricePerNight} kr
[white]Totalpris:[/] {totalPrice} kr
[white]Betald:[/] {paid}

[grey]Rumsdetaljer[/]
{features}
")
            {
                Header = new PanelHeader("[green]Bokningsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
        }

        // Dessa två ersätts av ErrorService
        public static void ShowError(string message)
        {
            ErrorService.Error(message);
        }

        public static void ShowSuccess(string message)
        {
            ErrorService.Success(message);
        }

        public static void ShowResult(bool success, string successMessage, string failMessage)
        {
            if (success)
                ErrorService.Success(successMessage);
            else
                ErrorService.Error(failMessage);
        }

        public static void ShowBookingTable(
            IEnumerable<Booking> bookings,
            Func<int, Guest?> getGuest,
            Func<int, Room?> getRoom)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Gäst[/]");
            table.AddColumn("[yellow]Rum[/]");
            table.AddColumn("[yellow]Datum[/]");
            table.AddColumn("[yellow]Totalpris[/]");
            table.AddColumn("[yellow]Betald[/]");

            foreach (var b in bookings)
            {
                var guest = getGuest(b.GuestId);
                var room = getRoom(b.RoomId);

                // Korrekt IsPaid-logik
                var paid = b.Invoice?.IsPaid == true
                    ? "[green]✔[/]"
                    : "[red]✘[/]";

                // Totalpris
                decimal total = 0;
                if (room != null)
                {
                    int nights = (b.ToDate - b.FromDate).Days;
                    if (nights < 1) nights = 1;
                    total = nights * room.PricePerNight;
                }

                table.AddRow(
                    b.Id.ToString(),
                    guest != null ? $"{guest.FirstName} {guest.LastName}" : "Okänd",
                    room != null ? room.RoomNumber.ToString() : "-",
                    $"{b.FromDate:yyyy-MM-dd} → {b.ToDate:yyyy-MM-dd}",
                    $"{total} kr",
                    paid
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
