using HotelBooking.Models;
using HotelBooking.Services;
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

        // Gemensam tabellmetod
        private void ShowInvoiceTable(IEnumerable<Invoice> invoices)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Bokning[/]");
            table.AddColumn("[yellow]Gäst[/]");
            table.AddColumn("[yellow]Belopp[/]");
            table.AddColumn("[yellow]Datum[/]");
            table.AddColumn("[yellow]Betald[/]");

            foreach (var inv in invoices)
            {
                table.AddRow(
                    inv.Id.ToString(),
                    inv.BookingId.ToString(),
                    inv.GuestId.ToString(),
                    $"{inv.TotalAmount} kr",
                    inv.CreatedDate.ToString("yyyy-MM-dd"),
                    inv.IsPaid ? "[green]Ja[/]" : "[red]Nej[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        // Visa alla fakturor
        public void ShowAll()
        {
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

        // Visa detaljer för en faktura
        public void ShowDetails()
        {
            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int id = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/]:");
            var invoice = _iinvoiceService.GetInvoice(id);

            if (invoice == null)
            {
                AnsiConsole.MarkupLine("[red]Faktura hittades inte.[/]");
                WaitForEsc();
                return;
            }

            var panel = new Panel($@"
[white]Faktura-ID:[/] {invoice.Id}
[white]Bokning-ID:[/] {invoice.BookingId}
[white]Gäst-ID:[/] {invoice.GuestId}
[white]Belopp:[/] {invoice.TotalAmount} kr
[white]Datum:[/] {invoice.CreatedDate:yyyy-MM-dd}
[white]Betald:[/] {(invoice.IsPaid ? "Ja" : "Nej")}
")
            {
                Header = new PanelHeader("[green]Fakturadetaljer[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            WaitForEsc();
        }

        // Skapa faktura
        public void Create()
        {
            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int bookingId = AnsiConsole.Ask<int>("Ange [yellow]boknings-ID[/]:");
            int guestId = AnsiConsole.Ask<int>("Ange [yellow]gäst-ID[/]:");
            decimal amount = AnsiConsole.Ask<decimal>("Ange [yellow]totalbelopp[/] (kr):");

            var invoice = _iinvoiceService.CreateInvoice(bookingId, guestId, amount);

            AnsiConsole.MarkupLine($"[green]Faktura skapad![/] ID: {invoice.Id}");
            WaitForEsc();
        }

        // Markera faktura som betald
        public void MarkAsPaid()
        {
            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int id = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/] att markera som betald:");

            bool success = _iinvoiceService.MarkAsPaid(id);

            AnsiConsole.MarkupLine(success
                ? "[green]Fakturan markerades som betald![/]"
                : "[red]Faktura hittades inte.[/]");

            WaitForEsc();
        }

        // Annullera obetalda bokningar 
        public void CancelUnpaid()
        {
            int cancelled = _iinvoiceService.CancelUnpaidBookings();

            if (cancelled == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Inga obetalda bokningar behövde annulleras.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{cancelled} bokningar annullerades pga obetalda fakturor.[/]");
            }

            WaitForEsc();
        }
    }
}