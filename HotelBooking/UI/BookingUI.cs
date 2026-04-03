using Spectre.Console;
using HotelBooking.Services;

namespace HotelBooking.UI
{
    public class BookingUI
    {
        private readonly BookingService _service;

        public BookingUI(BookingService service)
        {
            _service = service;
        }

        public void Create()
        {
            int guestId = AnsiConsole.Ask<int>("Gäst-ID:");
            int roomId = AnsiConsole.Ask<int>("Rum-ID:");

            DateOnly checkIn = AnsiConsole.Ask<DateOnly>("Incheckning (YYYY-MM-DD):");
            DateOnly checkOut = AnsiConsole.Ask<DateOnly>("Utcheckning (YYYY-MM-DD):");
            int guests = AnsiConsole.Ask<int>("Antal gäster:");

            var booking = _service.CreateBooking(
                guestId,
                roomId,
                checkIn.ToDateTime(TimeOnly.MinValue),
                checkOut.ToDateTime(TimeOnly.MinValue),
                guests
            );

            AnsiConsole.MarkupLine($"[green]Bokning skapad![/] ID: {booking.Id}");
            Console.ReadKey();
        }

        public void ShowDetails()
        {
            int id = AnsiConsole.Ask<int>("Ange boknings-ID:");

            var booking = _service.GetBooking(id);
            if (booking == null)
            {
                AnsiConsole.MarkupLine("[red]Bokning hittades inte[/]");
                Console.ReadKey();
                return;
            }

            var panel = new Panel($@"
ID: {booking.Id}
Gäst-ID: {booking.GuestId}
Rum-ID: {booking.RoomId}
Från: {booking.FromDate:yyyy-MM-dd}
Till: {booking.ToDate:yyyy-MM-dd}
Antal gäster: {booking.NumberOfGuests}
Bokad: {booking.BookingDate}
")
            {
                Header = new PanelHeader("[green]Bokningsdetaljer[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            Console.ReadKey();
        }

        public void ShowAll()
        {
            var bookings = _service.GetAll();

            if (!bookings.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga bokningar hittades[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table().RoundedBorder();

            table.AddColumn("ID");
            table.AddColumn("Gäst-ID");
            table.AddColumn("Rum-ID");
            table.AddColumn("Från");
            table.AddColumn("Till");
            table.AddColumn("Gäster");

            foreach (var b in bookings)
            {
                table.AddRow(
                    b.Id.ToString(),
                    b.GuestId.ToString(),
                    b.RoomId.ToString(),
                    b.FromDate.ToString("yyyy-MM-dd"),
                    b.ToDate.ToString("yyyy-MM-dd"),
                    b.NumberOfGuests.ToString()
                );
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        public void SoftDelete()
        {
            int id = AnsiConsole.Ask<int>("ID att ta bort:");

            if (_service.SoftDelete(id))
                AnsiConsole.MarkupLine("[green]Markerad som borttagen[/]");
            else
                AnsiConsole.MarkupLine("[red]Kunde inte ta bort[/]");

            Console.ReadKey();
        }

        public void Restore()
        {
            int id = AnsiConsole.Ask<int>("ID att återställa:");

            if (_service.Restore(id))
                AnsiConsole.MarkupLine("[green]Återställd[/]");
            else
                AnsiConsole.MarkupLine("[red]Kunde inte återställa[/]");

            Console.ReadKey();
        }

        public void Update()
        {
            int id = AnsiConsole.Ask<int>("ID att uppdatera:");

            var booking = _service.GetBooking(id);
            if (booking == null)
            {
                AnsiConsole.MarkupLine("[red]Bokning hittades inte[/]");
                Console.ReadKey();
                return;
            }

            string newGuest = AnsiConsole.Ask<string>("Nytt Gäst-ID (tomt = behåll):");
            string newFrom = AnsiConsole.Ask<string>("Ny incheckning (YYYY-MM-DD, tomt = behåll):");
            string newTo = AnsiConsole.Ask<string>("Ny utcheckning (YYYY-MM-DD, tomt = behåll):");
            string newGuests = AnsiConsole.Ask<string>("Nytt antal gäster (tomt = behåll):");

            _service.UpdateBooking(
                id,
                string.IsNullOrWhiteSpace(newGuest) ? null : int.Parse(newGuest),
                string.IsNullOrWhiteSpace(newFrom) ? null : DateTime.Parse(newFrom),
                string.IsNullOrWhiteSpace(newTo) ? null : DateTime.Parse(newTo),
                string.IsNullOrWhiteSpace(newGuests) ? null : int.Parse(newGuests)
            );

            AnsiConsole.MarkupLine("[green]Bokning uppdaterad[/]");
            Console.ReadKey();
        }
    }
}