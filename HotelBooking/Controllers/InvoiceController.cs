using HotelBooking.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{
    public class InvoiceController
    {
        private readonly InvoiceService _invoiceService;

        public InvoiceController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choise = UI.CrudMenuUI.Show("Hantera faktura");

                switch (choise)
                {
                    case 1:
                        _invoiceService.ShowAll();
                        break;
                    case 2:
                        _invoiceService.ShowDetails();
                        break;
                    case 3:
                        _invoiceService.Create();
                        break;
                    case 4:
                        _invoiceService.Update();
                        break;
                    case 5:
                        _invoiceService.SoftDelete();
                        break;
                    case 6:
                        _invoiceService.Restore();
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
