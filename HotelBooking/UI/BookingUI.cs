using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class BookingUI : EscUI
    {
        private readonly IBookingService _bookingService;
        private readonly IGuestService _guestService;
        private readonly IRoomService _roomService;

        public BookingUI(IBookingService bookingService, IGuestService guestService, IRoomService roomService)
        {
            _bookingService = bookingService;
            _guestService = guestService;
            _roomService = roomService;
        }

        // Gemensam tabellmetod
        private void ShowBookingTable(IEnumerable<Booking> bookings)
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
                var guest = _guestService.GetGuest(b.GuestId);
                var room = _roomService.GetRoom(b.RoomId);

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

        // Visa alla bokningar
        public void ShowAll()
        {
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowBookingTable(bookings);
            WaitForEsc();
        }

        // Visa detaljer
        public void ShowDetails()
        {
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowBookingTable(bookings);

            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/]:");
            var booking = _bookingService.GetBooking(id);

            if (booking == null)
            {
                AnsiConsole.MarkupLine("[red]Bokningen hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            var panel = new Panel($@"
[white]ID:[/] {booking.Id}

[white]Gäst:[/] {guest?.FullName ?? "Okänd"}
[white]Kontakt:[/] {guest?.Email ?? "-"} / {guest?.PhoneNumber ?? "-"}

[white]Rum:[/] {(room != null ? $"{room.RoomNumber} ({room.RoomType})" : booking.RoomId.ToString())}

[white]Från:[/] {booking.FromDate:yyyy-MM-dd}
[white]Till:[/] {booking.ToDate:yyyy-MM-dd}
[white]Antal gäster:[/] {booking.NumberOfGuests}

[white]Bokad:[/] {booking.BookingDate:yyyy-MM-dd HH:mm}
[white]Status:[/] {(booking.IsDeleted ? "[red]Raderad[/]" : "[green]Aktiv[/]")}
")
            {
                Header = new PanelHeader("[green]Bokningsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            WaitForEsc();
        }

        // Soft delete
        public void SoftDelete()
        {
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowBookingTable(bookings);

            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/] att ta bort:");
            bool success = _bookingService.SoftDelete(id);

            AnsiConsole.MarkupLine(success
                ? "[yellow]Bokningen är nu markerad som raderad.[/]"
                : "[red]Bokningen hittades inte.[/]");

            WaitForEsc();
        }

        // Restore
        public void Restore()
        {
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowBookingTable(bookings);

            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/] att återställa:");
            bool success = _bookingService.Restore(id);

            AnsiConsole.MarkupLine(success
                ? "[green]Bokningen är nu återställd![/]"
                : "[red]Bokningen hittades inte.[/]");

            WaitForEsc();
        }

        // Update
        public void Update()
        {
            var bookings = _bookingService.GetAll();
            ShowBookingTable(bookings);

            int id = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/] att uppdatera:");
            var booking = _bookingService.GetBooking(id);

            if (booking == null)
            {
                AnsiConsole.MarkupLine("[red]Bokningen hittades inte.[/]");
                WaitForEsc();
                return;
            }

            AnsiConsole.MarkupLine($@"
[green]Nuvarande värden:[/]
Gäst-ID: {booking.GuestId}
Rum-ID: {booking.RoomId}
Från: {booking.FromDate:yyyy-MM-dd}
Till: {booking.ToDate:yyyy-MM-dd}
Antal gäster: {booking.NumberOfGuests}
");

            string newGuest = AnsiConsole.Ask<string>($"Ny gäst-ID (tomt = {booking.GuestId}):");
            string newRoom = AnsiConsole.Ask<string>($"Nytt rum-ID (tomt = {booking.RoomId}):");
            string newFrom = AnsiConsole.Ask<string>($"Nytt från-datum (yyyy-MM-dd, tomt = {booking.FromDate:yyyy-MM-dd}):");
            string newTo = AnsiConsole.Ask<string>($"Nytt till-datum (yyyy-MM-dd, tomt = {booking.ToDate:yyyy-MM-dd}):");
            string newGuests = AnsiConsole.Ask<string>($"Nytt antal gäster (tomt = {booking.NumberOfGuests}):");

            int? guestId = string.IsNullOrWhiteSpace(newGuest) ? null : int.Parse(newGuest);
            int? roomId = string.IsNullOrWhiteSpace(newRoom) ? null : int.Parse(newRoom);
            DateTime? fromDate = string.IsNullOrWhiteSpace(newFrom) ? null : DateTime.Parse(newFrom);
            DateTime? toDate = string.IsNullOrWhiteSpace(newTo) ? null : DateTime.Parse(newTo);
            int? guests = string.IsNullOrWhiteSpace(newGuests) ? null : int.Parse(newGuests);

            bool success = _bookingService.UpdateBooking(id, guestId, fromDate, toDate, guests);

            AnsiConsole.MarkupLine(success
                ? "[green]Bokningen uppdaterades![/]"
                : "[red]Uppdatering misslyckades.[/]");

            WaitForEsc();
        }
    }
}