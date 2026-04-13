using Spectre.Console;
using HotelBooking.Services.Interfaces;
using HotelBooking.Models;

namespace HotelBooking.UI
{
    public class PaymentUI : EscUI
    {
        private readonly IPaymentService _service;

        public PaymentUI(IPaymentService service)
        {
            _service = service;
        }

        // Gemensam tabellmetod
        private void ShowPaymentTable(IEnumerable<Payment> payments)
        {
            var table = new Table().RoundedBorder();

            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Faktura-ID[/]");
            table.AddColumn("[yellow]Belopp[/]");
            table.AddColumn("[yellow]Datum[/]");
            table.AddColumn("[yellow]Metod[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var p in payments)
            {
                table.AddRow(
                    p.Id.ToString(),
                    p.InvoiceId.ToString(),
                    $"{p.Amount} kr",
                    p.PaymentDate.ToString("yyyy-MM-dd"),
                    p.Method.ToString(),
                    p.IsSuccessful ? "[green]Genomförd[/]" : "[red]Misslyckad[/]"
                );
            }

            AnsiConsole.Write(table);
        }

        // VISA BETALNINGAR PER FAKTURA
        public void Show(string subtitle)
        {
            BannerUI.PrintHeader("Hantera betalning", subtitle);

            var allPayments = _service.GetAllPayments();
            ShowPaymentTable(allPayments);

            int? invoiceId = AskIntOrEsc("Ange faktura-ID att filtrera på");
            if (invoiceId == null) return;

            if (invoiceId <= 0)
            {
                AnsiConsole.MarkupLine("[red]Faktura-ID måste vara ett positivt tal![/]");
                WaitForEsc();
                return;
            }

            var payments = _service.GetPaymentsForInvoice(invoiceId.Value);

            if (!payments.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga betalningar hittades för den fakturan.[/]");
                WaitForEsc();
                return;
            }

            ShowPaymentTable(payments);
            WaitForEsc();
        }

        // VISA ALLA BETALNINGAR
        public void ShowAll(string subtitle)
        {
            BannerUI.PrintHeader("Hantera betalning", subtitle);

            var payments = _service.GetAllPayments();

            if (!payments.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga betalningar hittades.[/]");
                WaitForEsc();
                return;
            }

            ShowPaymentTable(payments);
            WaitForEsc();
        }

        // SKAPA NY BETALNING
        public void Create(string subtitle)
        {
            AnsiConsole.Clear();
            BannerUI.ShowLogo();
            BannerUI.PrintHeader("Hantera betalning", subtitle);
            AnsiConsole.WriteLine();

            int? invoiceId = AskIntOrEsc("Ange faktura-ID");
            if (invoiceId == null) return;

            decimal? amount = AskDecimalOrEsc("Ange belopp (kr)");
            if (amount == null) return;

            string? method = AskWithEsc("Ange betalningsmetod (t.ex. Kort, Swish, Faktura)");
            if (method == null) return;

            bool confirm = BookingPrompts.Confirm("Vill du registrera denna betalning?");
            if (!confirm)
            {
                BookingRenderer.ShowError("Betalning avbröts.");
                WaitForEsc();
                return;
            }

            var payment = _service.CreatePayment(invoiceId.Value, amount.Value, method);

            BookingRenderer.ShowSuccess($"Betalning registrerad! ID: {payment.Id}");
            WaitForEsc();
        }
    }
}