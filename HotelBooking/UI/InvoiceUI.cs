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
        public void ShowAll(string subtitle)
        {
            BannerUI.PrintHeader("Hantera faktura", subtitle);

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

        // Visa detaljer
        public void ShowDetails(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int? id = AskIntOrEsc("Ange faktura-ID");
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
[white]Gäst:[/] {invoice.Guest?.FirstName} {invoice.Guest?.LastName}
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

        // Skapa faktura
        public void Create(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            int? bookingId = AskIntOrEsc("Ange boknings-ID");
            if (bookingId == null) return;

            int? guestId = AskIntOrEsc("Ange gäst-ID");
            if (guestId == null) return;

            decimal? amount = AskDecimalOrEsc("Ange totalbelopp (kr)");
            if (amount == null) return;

            bool confirm = BookingPrompts.Confirm("Skapa faktura med dessa uppgifter?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Skapande avbröts.");
                WaitForEsc();
                return;
            }

            var invoice = _iinvoiceService.CreateInvoice(bookingId.Value, guestId.Value, amount.Value);

            BookingRenderer.ShowSuccess($"Faktura skapad! ID: {invoice.Id}");
            WaitForEsc();
        }

        // Markera som betald
        public void MarkAsPaid(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera faktura", subtitle);
            AnsiConsole.WriteLine();

            var invoices = _iinvoiceService.GetAllInvoices();
            ShowInvoiceTable(invoices);

            int? id = AskIntOrEsc("Ange faktura-ID att markera som betald");
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

        // Annullera obetalda
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