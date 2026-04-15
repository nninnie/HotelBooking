using HotelBooking.UI;

namespace HotelBooking.Controllers
{
    public class InvoiceController
    {
        private readonly InvoiceUI _invoiceUI;

        public InvoiceController(InvoiceUI invoiceUI)
        {
            _invoiceUI = invoiceUI;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                string? choice = CrudMenuUI.Show(
                    "Hantera fakturor",
                    "Välj ett alternativ",
                    "Visa alla",
                    "Visa detaljer",
                    "Markera som betald",

                    "Tillbaka"
                );

                if (choice == null)
                    return;

                switch (choice)
                {
                    case "Visa alla":
                        _invoiceUI.ShowAll(choice);
                        break;

                    case "Visa detaljer":
                        _invoiceUI.ShowDetails(choice);
                        break;

                    case "Markera som betald":
                        _invoiceUI.MarkAsPaid(choice);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
    }
}