using HotelBooking.Services;
using HotelBooking.UI;

namespace HotelBooking.Controllers
{
    public class RoomController
    {
        private readonly RoomUI _roomUI;

        public RoomController(RoomUI roomUI)
        {
            _roomUI = roomUI;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choice = CrudMenuUI.Show("Hantera rum");

                switch (choice)
                {
                    case 1:
                        _roomUI.ShowAll();
                        break;
                    case 2:
                        _roomUI.ShowDetails();
                        break;
                    case 3:
                        _roomUI.Create();
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
                        return; // Tillbaka till huvudmenyn
                }
            }
        }
    }
}