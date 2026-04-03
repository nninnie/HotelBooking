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
            // 1. Hämta alla gäster
            var guests = _service.GetAllGuests(); 

            // 2. Låt användaren välja gäst
            var guestChoices = guests
                .Select(g => $"{g.Id}. {g.FullName} – {g.Email} – {g.PhoneNumber}")
                .ToList();

            guestChoices.Add("Skapa ny gäst");

            var selectedGuest = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [yellow]gäst[/]:")
                    .AddChoices(guestChoices)
            );

            int guestId;

            // 3. Om användaren vill skapa ny gäst
            if (selectedGuest == "Skapa ny gäst")
            {
                string fullName = AnsiConsole.Ask<string>("Fullständigt namn:");
                string email = AnsiConsole.Ask<string>("E-post:");
                string phone = AnsiConsole.Ask<string>("Telefonnummer:");

                var newGuest = _service.CreateGuest(fullName, email, phone); // Lägg till i BookingService
                guestId = newGuest.Id;

                AnsiConsole.MarkupLine($"[green]Ny gäst skapad![/] ID: {guestId}");
            }
            else
            {
                guestId = int.Parse(selectedGuest.Split('.')[0]);
            }

            // 4. Välj datum
            DateOnly checkIn = AnsiConsole.Ask<DateOnly>("Incheckning (YYYY-MM-DD):");
            DateOnly checkOut = AnsiConsole.Ask<DateOnly>("Utcheckning (YYYY-MM-DD):");

            // 5. Hämta lediga rum
            var availableRooms = _service.GetAvailableRooms(
                checkIn.ToDateTime(TimeOnly.MinValue),
                checkOut.ToDateTime(TimeOnly.MinValue)
            );

            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga lediga rum för dessa datum.[/]");
                Console.ReadKey();
                return;
            }

            // 6. Låt användaren välja rum
            var roomChoices = availableRooms
                .Select(r => $"{r.Id}. Rum {r.RoomNumber} – {r.RoomType} – {r.PricePerNight} kr/natt – Max {r.MaxGuests} gäster")
                .ToList();

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [yellow]ledigt rum[/]:")
                    .AddChoices(roomChoices)
            );

            int roomId = int.Parse(selectedRoom.Split('.')[0]);

            // 7. Beräkna pris
            var room = availableRooms.First(r => r.Id == roomId);
            int nights = (checkOut.ToDateTime(TimeOnly.MinValue) - checkIn.ToDateTime(TimeOnly.MinValue)).Days;
            decimal totalPrice = nights * room.PricePerNight;

            AnsiConsole.MarkupLine($@"
[green]Prisberäkning[/]
Pris per natt: {room.PricePerNight} kr
Antal nätter: {nights}
Totalpris: [yellow]{totalPrice} kr[/]
");

            // 8. Antal gäster
            int guestsCount = AnsiConsole.Ask<int>("Antal gäster:");

            if (guestsCount > room.MaxGuests)
            {
                AnsiConsole.MarkupLine("[red]För många gäster för detta rum.[/]");
                Console.ReadKey();
                return;
            }

            // 9. Skapa bokningen
            var booking = _service.CreateBooking(
                guestId,
                roomId,
                checkIn.ToDateTime(TimeOnly.MinValue),
                checkOut.ToDateTime(TimeOnly.MinValue),
                guestsCount
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