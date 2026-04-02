using HotelBooking.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Controllers
{
    public class RoomController
    {
        private readonly RoomService _roomService;

        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choise = UI.CrudMenuUI.Show("Hantera rum");

                switch (choise)
                {
                    case 1:
                        _roomService.ShowAll();
                        break;
                    case 2:
                        _roomService.ShowDetails();
                        break;
                    case 3:
                        _roomService.Create();
                        break;
                    case 4:
                        _roomService.Update();
                        break;
                    case 5:
                        _roomService.SoftDelete();
                        break;
                    case 6:
                        _roomService.Restore();
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
