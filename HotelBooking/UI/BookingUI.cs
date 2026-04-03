using Spectre.Console;
using HotelBooking.Services;

namespace HotelBooking.UI
{
    public class BookingUI
    {
        // BookingUI använder ett interface istället för en konkret klass (SOLID: DIP)
        private readonly IBookingService _service;

        public BookingUI(IBookingService service)
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

                var newGuest = _service.CreateGuest(fullName, email, phone);
                guestId = newGuest.Id;

                AnsiConsole.MarkupLine($"[green]Ny gäst skapad![/] ID: {guestId}");
            }
            else
            {
                guestId = int.Parse(selectedGuest.Split('.')[0]);
            }

            // 4. Välj datum via kalender
            var checkIn = ShowCalendarAndPickDate(DateTime.Today, DateTime.Today);
            var checkOut = ShowCalendarAndPickDate(checkIn.AddDays(1), checkIn.AddDays(1));

            // 5. Antal gäster (behövs för att filtrera rum)
            int guestsCount = AnsiConsole.Ask<int>("Antal gäster:");

            // 6. Hämta lediga rum baserat på datum + antal gäster
            var availableRooms = _service.GetAvailableRooms(checkIn, checkOut, guestsCount);

            if (!availableRooms.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga lediga rum för dessa datum.[/]");
                Console.ReadKey();
                return;
            }

            // 7. Låt användaren välja rum
            var roomChoices = availableRooms
                .Select(r => $"{r.Id}. Rum {r.RoomNumber} – {r.RoomType} – {r.PricePerNight} kr/natt – Max {r.MaxGuests} gäster")
                .ToList();

            var selectedRoom = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj [yellow]ledigt rum[/]:")
                    .AddChoices(roomChoices)
            );

            int roomId = int.Parse(selectedRoom.Split('.')[0]);
            var room = availableRooms.First(r => r.Id == roomId);

            // 8. Kontrollera att antal gäster får plats i rummet
            if (guestsCount > room.MaxGuests)
            {
                AnsiConsole.MarkupLine("[red]För många gäster för detta rum.[/]");
                Console.ReadKey();
                return;
            }

            // 9. Beräkna pris
            int nights = (checkOut - checkIn).Days;
            decimal totalPrice = nights * room.PricePerNight;

            AnsiConsole.MarkupLine($@"
[green]Prisberäkning[/]
Pris per natt: {room.PricePerNight} kr
Antal nätter: {nights}
Totalpris: [yellow]{totalPrice} kr[/]
");

            // 10. Skapa bokningen
            var booking = _service.CreateBooking(
                guestId,
                roomId,
                checkIn,
                checkOut,
                guestsCount
            );

            AnsiConsole.MarkupLine($"[green]Bokning skapad![/] ID: {booking.Id}");
            Console.ReadKey();
        }

        // --- Kalenderfunktioner (UI-ansvar, inte service) ---

        private DateTime ShowCalendarAndPickDate(DateTime startDate, DateTime minDate)
        {
            DateTime selectedDate = startDate;

            while (true)
            {
                AnsiConsole.Clear();
                RenderCalendar(selectedDate, minDate);

                var keyInfo = AnsiConsole.Console.Input.ReadKey(true);
                if (keyInfo == null)
                    continue;

                var key = keyInfo.Value.Key;

                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;

                    case ConsoleKey.LeftArrow:
                        selectedDate = selectedDate.AddDays(-1);
                        break;

                    case ConsoleKey.UpArrow:
                        selectedDate = selectedDate.AddDays(-7);
                        break;

                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);
                        break;

                    case ConsoleKey.Enter:
                        if (selectedDate >= minDate)
                            return selectedDate;
                        break;

                    case ConsoleKey.Escape:
                        return minDate;
                }

                if (selectedDate < minDate)
                    selectedDate = minDate;
            }
        }

        private void RenderCalendar(DateTime selectedDate, DateTime minDate)
        {
            var sw = new StringWriter();

            sw.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
            sw.WriteLine("Mån  Tis  Ons  Tor  Fre  Lör  Sön");
            sw.WriteLine("─────────────────────────────────");

            DateTime firstDay = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDay.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1;

            for (int i = 0; i < startDay; i++)
                sw.Write("     ");

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime current = new DateTime(selectedDate.Year, selectedDate.Month, day);

                if (current == selectedDate)
                    sw.Write($"[green]{day,2}[/]   ");
                else if (current < minDate)
                    sw.Write($"[grey]{day,2}[/]   ");
                else
                    sw.Write($"{day,2}   ");

                if ((startDay + day) % 7 == 0)
                    sw.WriteLine();
            }

            var panel = new Panel(sw.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
            AnsiConsole.MarkupLine("\nNavigera med [blue]piltangenter[/], välj med [green]Enter[/].");
        }
    }
}