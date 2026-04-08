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
                        _paymentUI.Create();    
                        break;
                    case 2:
                        _paymentUI.Show();      
                        break;
                    case 3:
                        _paymentUI.ShowAll();   
                        break;
                    case 4:
                        return;                 
                }
            }
        }
    }
}