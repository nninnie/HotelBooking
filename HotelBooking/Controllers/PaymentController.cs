using HotelBooking.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{
    public class PaymentController
    {

        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public void RunCrudMenu()
        {
            while (true)
            {
                int choise = UI.CrudMenuUI.Show("Hantera betalning");

                switch (choise)
                {
                    case 1:
                        _paymentService.ShowAll();
                        break;
                    case 2:
                        _paymentService.ShowDetails();
                        break;
                    case 3:
                        _paymentService.Create();
                        break;
                    case 4:
                        _paymentService.Update();
                        break;
                    case 5:
                        _paymentService.SoftDelete();
                        break;
                    case 6:
                        _paymentService.Restore();
                        break;
                    case 7:
                        return; // Tillbaka till huvudmenyn
                    default:
                        break;
                }
            }
        }
    }
}
