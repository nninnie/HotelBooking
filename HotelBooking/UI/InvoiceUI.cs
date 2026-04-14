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
            table.AddColumn("[yellow]Datum[/]");
            table.AddColumn("[yellow]Betald[/]");

            foreach (var inv in invoices)
            {
                table.AddRow(
                    inv.Id.ToString(),
                    inv.BookingId.ToString(),
                    $"{inv.Booking?.Guest?.FirstName} {inv.Booking?.Guest?.LastName}",
                    $"{inv.TotalAmount} kr",
                    inv.CreatedDate.ToString("yyyy-MM-dd"),
                    inv.IsPaid ? "[green]Ja[/]" : "[red]Nej[/]"
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

            int? id = AskIntOrEsc("Ange faktura-ID: ");
            if (id == null) return;

            var invoice = _iinvoiceService.GetInvoice(id.Value);
            if (invoice == null)
            {
                BookingRenderer.ShowError("Faktura hittades inte.");
                WaitForEsc();
                return;
            }

            var panel = new Panel($@"
[white]Faktura-ID:[/] {invoice.Id}
[white]Bokning-ID:[/] {invoice.BookingId}
[white]Gäst:[/] {invoice.Booking?.Guest?.FirstName} {invoice.Booking?.Guest?.LastName}
[white]Belopp:[/] {invoice.TotalAmount} kr
[white]Datum:[/] {invoice.CreatedDate:yyyy-MM-dd}
[white]Betald:[/] {(invoice.IsPaid ? "[green]Ja[/]" : "[red]Nej[/]")}
")
            {
                Header = new PanelHeader("[green]Fakturadetaljer[/]"),
                Border = BoxBorder.Rounded
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

            int? id = AskIntOrEsc("Ange faktura-ID att markera som betald: ");
            if (id == null) return;

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill markera fakturan som betald?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Åtgärden avbröts.");
                WaitForEsc();
                return;
            }

            bool success = _iinvoiceService.MarkAsPaid(id.Value);

            BookingRenderer.ShowResult(
                success,
                "Fakturan markerades som betald!",
                "Faktura hittades inte."
            );

            WaitForEsc();
        }

        // ANNULLERA OBETALDA 
        public void CancelUnpaid(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            bool confirm = BookingPrompts.Confirm("Är du säker att du vill annullera alla obetalda bokningar?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Åtgärden avbröts.");
                WaitForEsc();
                return;
            }

            int cancelled = _iinvoiceService.CancelUnpaidBookings();

            BookingRenderer.ShowResult(
                cancelled > 0,
                $"{cancelled} bokningar annullerades pga obetalda fakturor.",
                "Inga obetalda bokningar behövde annulleras."
            );

            WaitForEsc();
        }
    }
}