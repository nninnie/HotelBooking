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
            BannerUI.PrintHeader("Hantera betalning", subtitle);

            var payments = _service.GetAllPayments();
            ShowPaymentTable(payments);

            int? invoiceId = AskIntOrEsc("Ange faktura-ID");
            if (invoiceId == null) return;

            if (invoiceId <= 0)
            {
                AnsiConsole.MarkupLine("[red]Faktura-ID måste vara ett positivt tal![/]");
                WaitForEsc();
                return;
            }

            decimal amount;
            while (true)
            {
                amount = AnsiConsole.Ask<decimal>("Ange [yellow]belopp[/] (kr):");

                if (amount <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Belopp måste vara större än 0![/]");
                    continue;
                }

                break;
            }

            var payment = _service.CreatePayment(invoiceId.Value, amount);

            AnsiConsole.MarkupLine($"[green]Betalning registrerad![/] ID: {payment.Id}");
            WaitForEsc();
        }
    }
}