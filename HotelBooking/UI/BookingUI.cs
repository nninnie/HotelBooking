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
        private readonly InvoiceUI _invoiceUI;

        public BookingUI(IBookingService bookingService, IGuestService guestService, IRoomService roomService, InvoiceUI invoiceUI)
        {
            _bookingService = bookingService;
            _guestService = guestService;
            _roomService = roomService;
            _invoiceUI = invoiceUI;
        }

        // SKAPA BOKNING
        public void Create(string subtitle)
        {
            // HEADER
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Skapa ny bokning");
            AnsiConsole.WriteLine();

            // Välj eller skapa gäst
            var guestId = BookingPrompts.SelectOrCreateGuest(this, _guestService);
            if (guestId == null) return;

            // Välj datumintervall
            var (checkInDate, checkOutDate) = CalendarPicker.SelectDateRange();
            if (checkInDate == null || checkOutDate == null)
                return;

            if (checkInDate.Value.Date < DateTime.Today)
            {
                BookingRenderer.ShowError("Incheckningsdatum kan inte vara bakåt i tiden.");
                WaitForEsc();
                return;
            }

            if (checkOutDate.Value <= checkInDate.Value)
            {
                BookingRenderer.ShowError("Utcheckningsdatum måste vara efter incheckningsdatum!");
                WaitForEsc();
                return;
            }

            // Sätt standardtider
            var checkIn = checkInDate.Value.Date.AddHours(13);
            var checkOut = checkOutDate.Value.Date.AddHours(11);

            // Fråga antal gäster
            var guestsCount = BookingPrompts.AskGuestCount(this);
            if (guestsCount == null || guestsCount <= 0)
            {
                BookingRenderer.ShowError("Antal gäster måste vara minst 1.");
                WaitForEsc();
                return;
            }

            // Hämta ALLA lediga rum för datumintervallet
            var allRooms = _bookingService.GetAvailableRooms(
                checkIn,
                checkOut,
                guestsCount.Value
            );

            if (!allRooms.Any())
            {
                BookingRenderer.ShowError("Inga lediga rum för dessa datum.");
                WaitForEsc();
                return;
            }

            // Filtrera rum baserat på antal gäster
            var availableRooms = allRooms
                .Where(r => (r.Capacity + r.ExtraBeds) >= guestsCount.Value)
                .OrderBy(r => r.PricePerNight)
                .ToList();

            if (!availableRooms.Any())
            {
                BookingRenderer.ShowError("Inga rum klarar det antalet gäster.");
                WaitForEsc();
                return;
            }

            // Låt användaren välja rum (SelectRoom visar listan)
            var roomId = BookingPrompts.SelectRoom(this, availableRooms);
            if (roomId == null) return;

            var room = _roomService.GetRoom(roomId.Value);
            if (room == null)
            {
                BookingRenderer.ShowError("Kunde inte hämta rumsinformation.");
                WaitForEsc();
                return;
            }

            // Skapa bokningen
            var booking = _bookingService.CreateBooking(
                guestId.Value,
                roomId.Value,
                checkIn,
                checkOut,
                guestsCount.Value
            );

            // Bekräftelse
            int nights = booking.Nights;
            decimal totalPrice = booking.TotalPrice;

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]Bokningen är klar![/]");
            AnsiConsole.MarkupLine($"Rum: [yellow]{room.RoomNumber}[/] ({room.RoomType})");
            AnsiConsole.MarkupLine($"Incheckning: [yellow]{checkIn:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"Utcheckning: [yellow]{checkOut:yyyy-MM-dd HH:mm}[/]");
            AnsiConsole.MarkupLine($"Antal nätter: [yellow]{nights}[/]");
            AnsiConsole.MarkupLine($"Pris per natt: [yellow]{room.PricePerNight} kr[/]");
            AnsiConsole.MarkupLine($"Totalpris: [bold yellow]{totalPrice} kr[/]");
            AnsiConsole.WriteLine();

            AnsiConsole.MarkupLine("[blue]Faktura skapas...[/]");
            AnsiConsole.MarkupLine("[blue]Nyckelkort skapas...[/]");
            AnsiConsole.MarkupLine($"[green]✔ Nyckelkort till rum {room.RoomNumber} överlämnat till kunden.[/]");
            WaitForEsc();
        }

        // VISA ALLA
        public void ShowAll(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Visa alla");
            AnsiConsole.WriteLine();

            var bookings = _bookingService
                .GetAll();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            BookingRenderer.
                ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            WaitForEsc();
        }

        public void ShowDetails(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);
            AnsiConsole.WriteLine();

            // Hämta alla bokningar
            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            // Visa LISTAN
            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            // Fråga efter ID
            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            var booking = _bookingService.GetBooking(bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);
            AnsiConsole.WriteLine();

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            // Visa DETALJER 
            int nights = booking.Nights;
            if (nights < 1) nights = 1;

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            WaitForEsc();
        }


        public void SoftDelete(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Avboka");
            AnsiConsole.WriteLine();

            var bookings = _bookingService.GetAll();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            // ⭐ Kontrollera att bokningen finns i listan du visade
            var booking = bookings.FirstOrDefault(b => b.Id == bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            bool confirm = BookingPrompts.Confirm("\nÄr du säker att du vill ta bort denna bokning?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Avbokning avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.SoftDelete(bookingId.Value);

            BookingRenderer.ShowResult(
                success,
                "Bokningen är nu markerad som avbokad.",
                "Bokningen hittades inte."
            );

            WaitForEsc();
        }

        // RESTORE
        public void Restore(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Återställ bokning");
            AnsiConsole.WriteLine();

            var bookings = _bookingService.GetDeleted().ToList();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar hittades.");
                WaitForEsc();
                return;
            }

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            var booking = bookings.FirstOrDefault(b => b.Id == bookingId.Value);

            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", $"Återställ boknings ID: {bookingId.Value}");
            AnsiConsole.WriteLine();

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            // Beräkna nätter
            int nights = booking.Nights;

            if (nights < 1) nights = 1;

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            // BEKRÄFTA EFTER PANELEN
            bool confirm = BookingPrompts.Confirm("\nÄr du säker att du vill återställa denna bokning?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Återställning avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.Restore(bookingId.Value);

            BookingRenderer.ShowResult(
                success,
                "Bokningen är nu återställd!",
                "Bokningen hittades inte."
            );

            WaitForEsc();
        }


        // UPDATE
        public void Update(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", "Uppdatera bokning");
            AnsiConsole.WriteLine();

            var bookings = _bookingService.GetAll();

            BookingRenderer.ShowBookingTable(
                bookings,
                id => _guestService.GetGuest(id),
                id => _roomService.GetRoom(id)
            );

            var bookingId = BookingPrompts.AskBookingId(this);
            if (bookingId == null) return;

            if (bookingId <= 0)
            {
                BookingRenderer.ShowError("Ogiltigt boknings-ID.");
                WaitForEsc();
                return;
            }

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", $"Uppdatera boknings ID: {bookingId.Value}");
            AnsiConsole.WriteLine();

            var booking = _bookingService.GetBooking(bookingId.Value);
            if (booking == null)
            {
                BookingRenderer.ShowError("Bokningen hittades inte.");
                WaitForEsc();
                return;
            }

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            // Beräkna nätter
            int nights = booking.Nights;
            if (nights < 1) nights = 1;

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            // HÄR EFTER PANELEN FRÅGAR VI EFTER ÄNDRINGAR
            var updates = BookingPrompts.AskBookingUpdates(this, booking);
            if (updates == null) return;

            // Fallback till befintliga datum om användaren lämnar tomt
            var from = updates.FromDate ?? booking.FromDate;
            var to = updates.ToDate ?? booking.ToDate;

            // Lägg på standardtider
            from = from.Date.AddHours(13);
            to = to.Date.AddHours(11);

            // Spara tillbaka
            updates.FromDate = from;
            updates.ToDate = to;

            if (updates.FromDate >= updates.ToDate)
            {
                BookingRenderer.ShowError("Utcheckningsdatum måste vara efter incheckningsdatum.");
                WaitForEsc();
                return;
            }

            if (updates.NumberOfGuests <= 0)
            {
                BookingRenderer.ShowError("Antal gäster måste vara minst 1.");
                WaitForEsc();
                return;
            }

            bool success = _bookingService.UpdateBooking(
                booking.Id,
                updates.RoomId,
                updates.FromDate,
                updates.ToDate,
                updates.NumberOfGuests
            );


            BookingRenderer.ShowResult(
                success,
                "Bokningen uppdaterades!",
                "Uppdatering misslyckades."
            );

            WaitForEsc();
        }

        private Panel BuildBookingPanel(Booking booking, Guest? guest, Room? room)
        {
            int nights = booking.Nights;

            if (nights < 1) nights = 1;

            string status = booking.IsCancelled
                ? "[red]● Avbokad[/]"
                : booking.IsActive
                    ? "[green]● Aktiv[/]"
                    : "[yellow]● Avslutad[/]";

            return new Panel($@"[grey]Boknings-ID:[/] [white]{booking.Id}[/]

[bold dodgerblue1]Bokning[/]
[grey]Boknings-ID:[/] [white]{booking.Id}[/]
[grey]Skapad:[/] [white]{booking.BookingDate:yyyy-MM-dd HH:mm}[/]
[grey]Status:[/] {status}

[bold dodgerblue1]Gäst[/]
[grey]Namn:[/] [white]{guest?.FirstName} {guest?.LastName}[/]
[grey]E-post:[/] [white]{guest?.Email}[/]
[grey]Telefon:[/] [white]{guest?.PhoneNumber}[/]

[bold dodgerblue1]Rum[/]
[grey]Rum:[/] [white]{room?.RoomNumber} ({room?.RoomType})[/]
[grey]Pris per natt:[/] [white]{room?.PricePerNight} kr[/]

[bold dodgerblue1]Datum[/]
[grey]Incheckning:[/] [white]{booking.FromDate:yyyy-MM-dd HH:mm}[/]
[grey]Utcheckning:[/] [white]{booking.ToDate:yyyy-MM-dd HH:mm}[/]
[grey]Antal nätter:[/] [white]{nights}[/]

[bold dodgerblue1]Pris[/]
[grey]Totalpris:[/] [bold white]{booking.TotalPrice} kr[/]
")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };
        }

        public void CheckIn(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);

            var bookings = _bookingService.GetAll()
                .Where(b => !b.IsCheckedIn && !b.IsCompleted)
                .ToList();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga bokningar att checka in.");
                WaitForEsc();
                return;
            }

            var bookingId = BookingPrompts.SelectBookingFromList(this, bookings, subtitle);
            if (bookingId == null)
                return;

            // HÄMTA BOOKING FÖRST
            var booking = bookings.First(b => b.Id == bookingId.Value);

            // Visa detaljerad panel
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);

            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            // Check-in logik
            if (_bookingService.CheckIn(booking.Id))
            {
                var date = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                var donePanel = new Panel(
                    $"[black on green]✔ Gästen är nu incheckad[/]\n[grey]{date}[/]"
                )
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1)
                };

                AnsiConsole.Write(donePanel);
            }
            else
            {
                BookingRenderer.ShowError("Kunde inte checka in gästen.");
            }

            WaitForEsc();
        }

        public void CheckOut(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);

            var bookings = _bookingService.GetAll()
                .Where(b => b.IsCheckedIn)
                .ToList();

            if (!bookings.Any())
            {
                BookingRenderer.ShowError("Inga gäster är incheckade.");
                WaitForEsc();
                return;
            }

            var bookingId = BookingPrompts.SelectCheckedInBooking(this, _bookingService, subtitle);
            if (bookingId == null)
                return;

            var booking = bookings.First(b => b.Id == bookingId.Value);

            // Hämta gäst och rum
            var guest = _guestService.GetGuest(booking.GuestId);
            var room = _roomService.GetRoom(booking.RoomId);

            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);
            AnsiConsole.WriteLine();

            var panel = BuildBookingPanel(booking, guest, room);
            AnsiConsole.Write(panel);

            // Bekräftelse
            bool confirm = BookingPrompts.Confirm("Är du säker att du vill checka ut gästen?");
            if (!confirm)
                return;

            // Visa “Gäst utcheckad”
            var donePanel = new Panel("[green]✔ Gäst utcheckad[/]")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1)
            };
            AnsiConsole.Write(donePanel);

            // Fakturafråga
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[white]Vill du betala fakturan nu?[/]");
            AnsiConsole.MarkupLine("[grey](j = ja, ENTER = nej, ESC = avbryt)[/]");

            bool? pay = AskBoolOrEsc(defaultValue: false);
            if (pay == null)
                return;

            // Extra säkerhetsfråga
            if (pay == true)
            {
                bool confirmInvoice = BookingPrompts.Confirm("Är du säker på att du vill markera fakturan som betald?");
                if (!confirmInvoice)
                    pay = false;
            }

            // Rensa och visa utcheckad-panel igen
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera bokningar", subtitle);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(donePanel);
            AnsiConsole.WriteLine();

            // Utför utcheckning
            if (_bookingService.CheckOut(booking.Id, pay.Value))
            {
                // Hämta om bokningen så PaidDate och Invoice är uppdaterade
                if (booking == null)
                {
                    BookingRenderer.ShowError("Kunde inte ladda bokningen.");
                    return;
                }

                // Hämta gäst
                guest = _guestService.GetGuest(booking.GuestId);

                if (guest == null)
                {
                    BookingRenderer.ShowError("Kunde inte ladda gästens information.");
                    return;
                }
     


                room = _roomService.GetRoom(booking.RoomId);
                if (room == null)
                {
                    BookingRenderer.ShowError("Kunde inte ladda rummets information.");
                    return;
                }

            }
            else
            {
                BookingRenderer.ShowError("Kunde inte checka ut gästen.");
                WaitForEsc();
                return;
            }

            // Visa mini-faktura om betald
            if (pay == true)
            {
                if (booking?.Invoice == null)
                {
                    BookingRenderer.ShowError("Ingen faktura hittades för bokningen.");
                    return;
                }

                if (guest == null || room == null)
                {
                    BookingRenderer.ShowError("Kunde inte ladda gästens eller rummets information.");
                    return;
                }

                var inv = booking.Invoice;
                int nights = (booking.ToDate - booking.FromDate).Days;

                string paidDate = inv.PaidDate?.ToString("yyyy-MM-dd") ?? "-";
                string status = inv.IsCancelled
                    ? "[yellow]● Annullerad[/]"
                    : inv.IsPaid
                        ? "[green]● Betald[/]"
                        : "[red]● Obetald[/]";

                var invoicePanel = new Panel($@"
[bold dodgerblue1]Faktura[/]
[grey]Faktura-ID:[/] [white]{inv.Id}[/]
[grey]Bokning-ID:[/] [white]{inv.BookingId}[/]

[bold dodgerblue1]Gäst[/]
[grey]Namn:[/] [white]{guest.FirstName} {guest.LastName}[/]
[grey]E-post:[/] [white]{guest.Email}[/]
[grey]Telefon:[/] [white]{guest.PhoneNumber}[/]

[bold dodgerblue1]Bokning[/]
[grey]Rum:[/] [white]{room.RoomNumber} ({room.RoomType})[/]
[grey]Period:[/] [white]{booking.FromDate:yyyy-MM-dd} → {booking.ToDate:yyyy-MM-dd}[/]
[grey]Antal nätter:[/] [white]{nights}[/]
[grey]Pris per natt:[/] [white]{room.PricePerNight} kr[/]

[bold dodgerblue1]Fakturainfo[/]
[grey]Skapad:[/] [white]{inv.CreatedDate:yyyy-MM-dd}[/]
[grey]Förfallodatum:[/] [white]{inv.DueDate:yyyy-MM-dd}[/]
[grey]Betald datum:[/] [white]{paidDate}[/]

[grey]Totalbelopp:[/] [bold white]{inv.TotalAmount} kr[/]
[grey]Status:[/] {status}
")
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1)
                };

                AnsiConsole.WriteLine();
                AnsiConsole.Write(invoicePanel);
            }

            WaitForEsc();
        }


    }
}
