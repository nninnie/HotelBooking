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
                string choice = CrudMenuUI.Show("Hantera rum", "Välj ett alternativ");

                switch (choice)
                {
                    case "Skapa ny":
                        _roomUI.Create(choice);
                        break;

                    case "Visa alla":
                        _roomUI.ShowAll(choice);
                        break;

                    case "Visa detaljer":
                        _roomUI.ShowDetails(choice);
                        break;

                    case "Uppdatera":
                        _roomUI.Update(choice);
                        break;

                    case "Ta bort":
                        _roomUI.SoftDelete(choice);
                        break;

                    case "Återställ":
                        _roomUI.Restore(choice);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
    }
}