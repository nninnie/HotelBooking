using Autofac.Core;
using HotelBooking.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{
    public class BookingController
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public void CreateDirect()
        {
            _bookingService.Create(); // “Ny bokning” från huvudmenyn
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choice = UI.CrudMenuUI.Show("Hantera bokningar");
                switch (choice)
                {
                    case 1:
                        _bookingService.ShowAll();
                        break;
                    case 2:
                        _bookingService.ShowDetails();
                        break;
                    case 3:
                        _bookingService.Create();
                        break;
                    case 4:
                        _bookingService.Update();
                        break;
                    case 5:
                        _bookingService.SoftDelete();
                        break;
                    case 6:
                        _bookingService.Restore();
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
