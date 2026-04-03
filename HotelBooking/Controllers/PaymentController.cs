using Autofac.Core;
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
                int choice = CrudMenuUI.Show("Hantera betalning");

                switch (choice)
                {
                    case 1:
                        _paymentUI.Show();       // Visa betalning via faktura-ID
                        break;
                    case 2:
                        _paymentUI.Create();     // Om du vill lägga till Create i PaymentUI
                        break;
                    case 3:
                        _paymentUI.ShowAll();    // Om du vill lägga till ShowAll i PaymentUI
                        break;
                    case 4:
                        return;                  // Tillbaka till huvudmenyn
                }
            }
        }
    }
}