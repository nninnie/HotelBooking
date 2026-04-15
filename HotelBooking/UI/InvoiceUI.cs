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

            int? id = AskIntOrEsc("Ange faktura-ID: ");
            if (id == null) return;

            var invoice = _iinvoiceService.GetInvoice(id.Value);
            if (invoice == null)
            {
                BookingRenderer.ShowError("Faktura hittades inte.");
                WaitForEsc();
                return;
            }

            string status = invoice.IsCancelled
                ? "[grey]Annullerad[/]"
                : invoice.IsPaid
                    ? "[green]Betald[/]"
                    : "[red]Obetald[/]";

            var panel = new Panel($@"
[white]Faktura-ID:[/] {invoice.Id}
[white]Bokning-ID:[/] {invoice.BookingId}
[white]Gäst:[/] {invoice.Booking?.Guest?.FirstName} {invoice.Booking?.Guest?.LastName}
[white]Belopp:[/] {invoice.TotalAmount} kr
[white]Datum:[/] {invoice.CreatedDate:yyyy-MM-dd}
[white]Status:[/] {status}
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

            var invoice = _iinvoiceService.GetInvoice(id.Value);
            if (invoice == null)
            {
                BookingRenderer.ShowError("Faktura hittades inte.");
                WaitForEsc();
                return;
            }

            // ❗ Stoppa betalning av annullerad faktura
            if (invoice.IsCancelled)
            {
                BookingRenderer.ShowError("Fakturan är annullerad och kan inte betalas.");
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

            BookingRenderer.ShowResult(
                success,
                "Fakturan markerades som betald!",
                "Faktura hittades inte eller kunde inte markeras som betald."
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

            // Visa endast obetalda & ej annullerade fakturor
            var unpaid = _iinvoiceService
                .GetAllInvoices()
                .Where(i => !i.IsPaid && !i.IsCancelled)
                .ToList();

            if (!unpaid.Any())
            {
                BookingRenderer.ShowError("Inga obetalda fakturor att annullera.");
                WaitForEsc();
                return;
            }

            ShowInvoiceTable(unpaid);

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
