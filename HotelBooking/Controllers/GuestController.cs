using HotelBooking.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{
    public class GuestController
    {
        private readonly GuestService _guestService;

        public GuestController(GuestService guestService)
        {
            _guestService = guestService;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choice = UI.CrudMenuUI.Show("Hantera gäster");
                switch (choice)
                {
                    case 1:
                        _guestService.ShowAll();
                        break;
                    case 2:
                        _guestService.ShowDetails();
                        break;
                    case 3:
                        _guestService.Create();
                        break;
                    case 4:
                        _guestService.Update();
                        break;
                    case 5:
                        _guestService.SoftDelete();
                        break;
                    case 6:
                        _guestService.Restore();
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
