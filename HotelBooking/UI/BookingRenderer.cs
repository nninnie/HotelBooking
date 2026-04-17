using HotelBooking.Models;
using HotelBooking.Core;
using Spectre.Console;

namespace HotelBooking.UI
{
    public static class BookingRenderer
    {
        public static Panel ShowBookingDetails(Booking booking, Guest? guest, Room? room)
        {
            int nights = (booking.ToDate.Date - booking.FromDate.Date).Days;
            if (nights < 1) nights = 1;

            decimal pricePerNight = room?.PricePerNight ?? 0;
            decimal totalPrice = nights * pricePerNight;

            string status =
                booking.IsCancelled ? "[red]● Avbokad[/]" :
                booking.IsActive ? "[green]● Aktiv[/]" :
                booking.IsCheckedIn ? "[blue]● Incheckad[/]" :
                booking.IsCompleted ? "[grey]● Slutförd[/]" :
                "[yellow]● Kommande[/]";

            string paid = booking.Invoice?.IsPaid == true
                ? "[green]✔ Betald[/]"
                : "[red]✘ Obetald[/]";

            string features = room == null
                ? "-"
                : $@"Kapacitet: {room.Capacity}
Extrabäddar: {room.ExtraBeds}
Havsutsikt: {(room.HasSeaView ? "Ja" : "Nej")}
Minibar: {(room.HasMinibar ? "Ja" : "Nej")}
Husdjur tillåtet: {(room.IsPetFriendly ? "Ja" : "Nej")}
Balkong: {(room.HasBalcony ? "Ja" : "Nej")}
Badkar: {(room.HasBathtub ? "Ja" : "Nej")}";

            return new Panel($@"
[bold dodgerblue1]Bokning[/]
[grey]ID:[/] [white]{booking.Id}[/]
[grey]Skapad:[/] [white]{booking.BookingDate:yyyy-MM-dd HH:mm}[/]
[grey]Status:[/] {status}

[bold dodgerblue1]Gäst[/]
[grey]Namn:[/] [white]{guest?.FirstName} {guest?.LastName}[/]
[grey]Kontakt:[/] [white]{guest?.Email ?? "-"} / {guest?.PhoneNumber ?? "-"}[/]

[bold dodgerblue1]Rum[/]
[grey]Rum:[/] [white]{room?.RoomNumber} ({room?.RoomType})[/]
[grey]Pris per natt:[/] [white]{pricePerNight} kr[/]

[bold dodgerblue1]Datum[/]
[grey]Incheckning:[/] [white]{booking.FromDate:yyyy-MM-dd HH:mm}[/]
[grey]Utcheckning:[/] [white]{booking.ToDate:yyyy-MM-dd HH:mm}[/]
[grey]Antal nätter:[/] [white]{nights}[/]
[grey]Antal gäster:[/] [white]{booking.NumberOfGuests}[/]

[bold dodgerblue1]Pris[/]
[grey]Totalpris:[/] [bold white]{totalPrice} kr[/]
[grey]Betald:[/] {paid}

[bold dodgerblue1]Rumsdetaljer[/]
{features}
")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };
        }



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
            table.AddColumn("[yellow]Status[/]");

            foreach (var b in bookings)
            {
                var guest = getGuest(b.GuestId);
                var room = getRoom(b.RoomId);

                var paid = b.Invoice?.IsPaid == true
                    ? "[green]✔[/]"
                    : "[red]✘[/]";

                decimal total = 0;
                if (room != null)
                {
                    int nights = (b.ToDate - b.FromDate).Days;
                    if (nights < 1) nights = 1;
                    total = nights * room.PricePerNight;
                }

                string status =
                    b.IsCancelled ? "[red]Avbokad[/]" :
                    b.IsCompleted ? "[grey]Slutförd[/]" :
                    b.IsCheckedIn ? "[green]Incheckad[/]" :
                    DateTime.Today < b.FromDate ? "[yellow]Kommande[/]" :
                    DateTime.Today > b.ToDate ? "[grey]Utcheckad[/]" :
                    "[blue]Aktiv[/]";

                table.AddRow(
                    b.Id.ToString(),
                    guest != null ? $"{guest.FirstName} {guest.LastName}" : "Okänd",
                    room != null ? room.RoomNumber.ToString() : "-",
                    $"{b.FromDate:yyyy-MM-dd} → {b.ToDate:yyyy-MM-dd}",
                    $"{total} kr",
                    paid,
                    status
                );
            }

            AnsiConsole.Write(table);
        }
        }
    }
