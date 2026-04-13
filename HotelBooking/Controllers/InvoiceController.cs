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
                // CrudMenuUI returnerar TEXT
                string choice = CrudMenuUI.Show("Hantera faktura", "Välj ett alternativ");

                switch (choice)
                {
                    case "Skapa ny":
                        _invoiceUI.Create(choice);
                        break;

                    case "Visa alla":
                        _invoiceUI.ShowAll(choice);
                        break;

                    case "Visa detaljer":
                        _invoiceUI.ShowDetails(choice);
                        break;

                    case "Uppdatera":
                        _invoiceUI.MarkAsPaid(choice);
                        break;

                    case "Ta bort":
                        _invoiceUI.CancelUnpaid(choice);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
    }
}