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
                int choice = CrudMenuUI.Show("Hantera faktura");

                switch (choice)
                {
                    case 1:
                        _invoiceUI.ShowAll();
                        break;
                    case 2:
                        _invoiceUI.ShowDetails();
                        break;
                    case 3:
                        _invoiceUI.Create();
                        break;
                    case 4:
                        _invoiceUI.MarkAsPaid();
                        break;
                    case 5:
                        return; // Tillbaka till huvudmenyn
                }
            }
        }
    }
}