using HotelBooking.Services;
using HotelBooking.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{
    public class GuestController
    {
        private readonly GuestUI _guestUI;

        public GuestController(GuestUI guestUI)
        {
            _guestUI = guestUI;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choice = UI.CrudMenuUI.Show("Hantera gäster");

                switch (choice)
                {
                    case 1:
                        _guestUI.Create();
                        break;
                    case 2:
                        _guestUI.ShowAll();
                        break;
                    case 3:
                        _guestUI.ShowDetails();
                        break;
                    case 4:
                        _guestUI.Update();
                        break;
                    case 5:
                        return; 
                }
            }
        }
    }
}
