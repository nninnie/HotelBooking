using HotelBooking.Services;
using Spectre.Console;

namespace HotelBooking.UI
{
    public class PaymentUI
    {
        private readonly PaymentService _service;

        public PaymentUI(PaymentService service)
        {
            _service = service;
        }

        public void Show()
        {
            int invoiceId = AnsiConsole.Ask<int>("Ange [yellow]faktura-ID[/]:");

            var payment = _service.GetPaymentByInvoiceId(invoiceId);

            if (payment == null)
            {
                AnsiConsole.MarkupLine("[red]Ingen betalning hittades för den fakturan.[/]");
                Console.ReadKey();
                return;
            }

            var panel = new Panel($@"
[white]Betalnings-ID:[/] {payment.Id}
[white]Faktura-ID:[/] {payment.InvoiceId}
[white]Belopp:[/] {payment.Amount} kr
[white]Datum:[/] {payment.PaymentDate:yyyy-MM-dd}
[white]Metod:[/] {payment.PaymentMethod}
[white]Status:[/] {(payment.IsSuccessful ? "Genomförd" : "Misslyckad")}
")
            {
                Header = new PanelHeader("[green]Betalningsinformation[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
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