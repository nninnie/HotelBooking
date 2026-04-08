using HotelBooking.Services.Interfaces;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class PaymentUI
    {
        private readonly IPaymentService _service;

        public PaymentUI(IPaymentService service)
        {
            _service = service;
        }

        public void Show()
        {
            int invoiceId = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/]:");

            var payments = _service.GetPaymentsForInvoice(invoiceId);

            if (!payments.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga betalningar hittades för den fakturan.[/]");
                Console.ReadKey();
                return;
            }

            var table = new Table().RoundedBorder();
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Belopp[/]");
            table.AddColumn("[yellow]Datum[/]");
            table.AddColumn("[yellow]Metod[/]");
            table.AddColumn("[yellow]Status[/]");

            foreach (var p in payments)
            {
                table.AddRow(
                    p.Id.ToString(),
                    $"{p.Amount} kr",
                    p.PaymentDate.ToString("yyyy-MM-dd"),
                    p.PaymentMethod ?? "Okänd",
                    p.IsSuccessful ? "[green]Genomförd[/]" : "[red]Misslyckad[/]"
                );
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }

        public void ShowAll()
        {
            var payments = _service.GetAllPayments();

            if (!payments.Any())
            {
                AnsiConsole.MarkupLine("[red]Inga betalningar hittades.[/]");
                Console.ReadKey();
                return;
            }

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
                    p.PaymentMethod ?? "Okänd",
                    p.IsSuccessful ? "[green]Genomförd[/]" : "[red]Misslyckad[/]"
                );
            }

            AnsiConsole.Write(table);
            Console.ReadKey();
        }
        public void Create()
        {
            int invoiceId = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/]:");
            decimal amount = AnsiConsole.Ask<decimal>("Ange [yellow]belopp[/] (kr):");
            string method = AnsiConsole.Ask<string>("Ange [yellow]betalningsmetod[/] (t.ex. Kort, Swish, Faktura):");

            var payment = _service.CreatePayment(invoiceId, amount, method);

            AnsiConsole.MarkupLine($"[green]Betalning skapad![/] ID: {payment.Id}");
            Console.ReadKey();
        }

    }
}