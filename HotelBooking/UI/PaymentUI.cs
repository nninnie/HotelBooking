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

        public void Show()
        {
            CurrentHeader = "BETALNINGAR PER FAKTURA";

            // Visa alla betalningar först
            var allPayments = _service.GetAllPayments();
            ShowPaymentTable(allPayments);

            int invoiceId;
            while (true)
            {
                invoiceId = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/] att filtrera på:");

                // VALIDATION
                if (invoiceId <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Faktura-ID måste vara ett positivt tal![/]");
                    continue;
                }

                break;
            }

            var payments = _service.GetPaymentsForInvoice(invoiceId);

            if (!payments.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga betalningar hittades för den fakturan.[/]");
                WaitForEsc();
                return;
            }

            ShowPaymentTable(payments);
            WaitForEsc();
        }

        public void ShowAll()
        {
            CurrentHeader = "ALLA BETALNINGAR";

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

        public void Create()
        {
            CurrentHeader = "REGISTRERA BETALNING";

            var payments = _service.GetAllPayments();
            ShowPaymentTable(payments);

            int invoiceId;
            while (true)
            {
                invoiceId = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/]:");

                // VALIDATION
                if (invoiceId <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Faktura-ID måste vara ett positivt tal![/]");
                    continue;
                }

                break;
            }

            decimal amount;
            while (true)
            {
                amount = AnsiConsole.Ask<decimal>("Ange [yellow]belopp[/] (kr):");

                // VALIDATION
                if (amount <= 0)
                {
                    AnsiConsole.MarkupLine("[red]Belopp måste vara större än 0![/]");
                    continue;
                }

                break;
            }

            var payment = _service.CreatePayment(invoiceId, amount);

            AnsiConsole.MarkupLine($"[green]Betalning registrerad![/] ID: {payment.Id}");
            WaitForEsc();
        }
    }
}