using HotelBooking.Services.Interfaces;
using HotelBooking.UI;

namespace HotelBooking.Controllers
{
    public class RoomController
    {
        private readonly RoomUI _roomUI;
        private readonly IRoomService _roomService;

        public RoomController(RoomUI roomUI, IRoomService roomService)
        {
            _roomUI = roomUI;
            _roomService = roomService;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                var rooms = _roomService.GetAllRooms();
                int choice = CrudMenuUI.Show("Hantera rum", rooms);

                switch (choice)
                {
                    case 1:
                        _roomUI.Create();
                        break;
                    case 2:
                        _roomUI.ShowAll();
                        break;
                    case 3:
                        _roomUI.ShowDetails();
                        break;
                    case 4:
                        _roomUI.Update();
                        break;
                    case 5:
                        _roomUI.SoftDelete();
                        break;
                    case 6:
                        _roomUI.Restore();
                        break;
                    case 7:
                        return; 
                }
            }
        }
    }
}