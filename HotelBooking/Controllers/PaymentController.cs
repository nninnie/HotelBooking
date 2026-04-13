using HotelBooking.UI;
using Spectre.Console;

namespace HotelBooking.Controllers
{
    public class PaymentController
    {
        private readonly PaymentUI _paymentUI;

        public PaymentController(PaymentUI paymentUI)
        {
            _paymentUI = paymentUI;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                string? choice = CrudMenuUI.Show(
                    "Hantera betalningar",
                    "Välj ett alternativ",
                    "Skapa ny",
                    "Visa alla",
                    "Visa detaljer",
                    "Tillbaka"
                );

                if (choice == null)
                    return; 

                switch (choice)
                {
                    case "Skapa ny":
                        _paymentUI.Create(choice);
                        break;

                    case "Visa alla":
                        _paymentUI.ShowAll(choice);
                        break;

                    case "Visa detaljer":
                        _paymentUI.Show(choice);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
    }
}