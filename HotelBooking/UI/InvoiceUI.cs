using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class InvoiceUI : EscUI
    {
        private readonly IInvoiceService _iinvoiceService;

        public InvoiceUI(IInvoiceService iinvoiceService)
        {
            _iinvoiceService = iinvoiceService;
        }

        // TABELL
        private void ShowInvoiceTable(IEnumerable<Invoice> invoices)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Bokning[/]");
            table.AddColumn("[yellow]Gäst[/]");
            table.AddColumn("[yellow]Belopp[/]");
            table.AddColumn("[yellow]Skapad[/]");
            table.AddColumn("[yellow]Betald[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var inv in invoices)
            {
                string status = inv.IsCancelled
                    ? "[grey]Annullerad[/]"
                    : inv.IsPaid
                        ? "[green]Betald[/]"
                        : "[red]Obetald[/]";

                table.AddRow(
                    inv.Id.ToString(),
                    inv.BookingId.ToString(),
                    $"{inv.Booking?.Guest?.FirstName} {inv.Booking?.Guest?.LastName}",
                    $"{inv.TotalAmount} kr",
                    inv.CreatedDate.ToString("yyyy-MM-dd"),
                    inv.PaidDate?.ToString("yyyy-MM-dd") ?? "-",
                    status
                );
            }

            AnsiConsole.Write(table);
        }

        // VISA ALLA
        public void ShowAll(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            var invoices = _iinvoiceService.GetAllInvoices();

            if (!invoices.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga fakturor hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowInvoiceTable(invoices);
            WaitForEsc();
        }

        // VISA DETALJER
        public void ShowDetails(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int? id = AskIntOrEsc("\nAnge faktura-ID: ");
            if (id == null) return;

            var invoice = _iinvoiceService.GetInvoice(id.Value);
            if (invoice == null)
            {
                BookingRenderer.ShowError("Faktura hittades inte.");
                WaitForEsc();
                return;
            }

            var booking = invoice.Booking;
            var guest = booking?.Guest;
            var room = booking?.Room;

            int nights = (int)(booking!.ToDate - booking.FromDate).TotalDays;
            if (nights < 1) nights = 1;

            string status = invoice.IsCancelled
                ? "[red]● Annullerad[/]"
                : invoice.IsPaid
                    ? "[green]● Betald[/]"
                    : "[yellow]● Obetald[/]";

            var panel = new Panel($@"
[bold dodgerblue1]Faktura[/]
[grey]Faktura-ID:[/] [white]{invoice.Id}[/]
[grey]Bokning-ID:[/] [white]{invoice.BookingId}[/]

[bold dodgerblue1]Gäst[/]
[grey]Namn:[/] [white]{guest?.FirstName} {guest?.LastName}[/]
[grey]E-post:[/] [white]{guest?.Email}[/]
[grey]Telefon:[/] [white]{guest?.PhoneNumber}[/]

[bold dodgerblue1]Bokning[/]
[grey]Rum:[/] [white]{room?.RoomNumber} ({room?.RoomType})[/]
[grey]Period:[/] [white]{booking.FromDate:yyyy-MM-dd} → {booking.ToDate:yyyy-MM-dd}[/]
[grey]Antal nätter:[/] [white]{nights}[/]
[grey]Pris per natt:[/] [white]{room?.PricePerNight} kr[/]

[bold dodgerblue1]Fakturainfo[/]
[grey]Skapad:[/] [white]{invoice.CreatedDate:yyyy-MM-dd}[/]
[grey]Förfallodatum:[/] [white]{invoice.DueDate:yyyy-MM-dd}[/]
[grey]Betald datum:[/] [white]{(invoice.PaidDate?.ToString("yyyy-MM-dd") ?? "-")}[/]

[grey]Totalbelopp:[/] [bold white]{invoice.TotalAmount} kr[/]
[grey]Status:[/] {status}
")
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };

            AnsiConsole.Write(panel);

            WaitForEsc();
        }


        // MARKERA SOM BETALD
        public void MarkAsPaid(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int? id = AskIntOrEsc("\nAnge faktura-ID att markera som betald: ");
            if (id == null) return;

            var invoice = _iinvoiceService.GetInvoice(id.Value);
            if (invoice == null)
            {
                BookingRenderer.ShowError("Faktura hittades inte.");
                WaitForEsc();
                return;
            }

            // Stoppa betalning av annullerad faktura
            if (invoice.IsCancelled)
            {
                BookingRenderer.ShowError("Bokningen är avbokad. Därför är fakturan annullerad och kan inte betalas.");
                WaitForEsc();
                return;
            }

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill markera fakturan som betald?");

            if (!confirm)
            {
                BookingRenderer.ShowError("Åtgärden avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _iinvoiceService.MarkAsPaid(id.Value);

            // Faktura
            if (success)
            {
                // Hämta om fakturan så PaidDate är uppdaterad
                invoice = _iinvoiceService.GetInvoice(id.Value);

                if (invoice == null)
                {
                    BookingRenderer.ShowError("Fakturan kunde inte laddas efter betalning.");
                    WaitForEsc();
                    return;
                }

                // Null-säkra relaterade objekt
                if (invoice.Booking == null || invoice.Booking.Guest == null || invoice.Booking.Room == null)
                {
                    BookingRenderer.ShowError("Kunde inte ladda boknings-, gäst- eller rumsinformation.");
                    WaitForEsc();
                    return;
                }

                var booking = invoice.Booking;
                var guest = booking.Guest;
                var room = booking.Room;

                string paidDate = invoice.PaidDate?.ToString("yyyy-MM-dd") ?? "-";
                string status = invoice.IsPaid ? "[green]Betald[/]" : "[red]Obetald[/]";

                var invoiceText =
                    $"[white]Faktura-ID:[/] {invoice.Id}\n" +
                    $"[white]Boknings-ID:[/] {booking.Id}\n" +
                    $"[white]Gäst:[/] {Markup.Escape(guest.FirstName)} {Markup.Escape(guest.LastName)}\n" +
                    $"[white]Rum:[/] {room.RoomNumber}\n" +
                    $"[white]Totalbelopp:[/] {invoice.TotalAmount} kr\n" +
                    $"[white]Skapad:[/] {invoice.CreatedDate:yyyy-MM-dd}\n" +
                    $"[white]Betald datum:[/] {paidDate}\n" +
                    $"[white]Status:[/] {status}";

                var invoicePanel = new Panel(invoiceText)
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1),
                    Header = new PanelHeader("Faktura")
                };

                AnsiConsole.WriteLine();
                AnsiConsole.Write(invoicePanel);
            }

            BookingRenderer.ShowResult(
                success,
                "Fakturan markerades som betald!",
                "Faktura hittades inte eller kunde inte markeras som betald."
            );

            WaitForEsc();
        }


    }
}
