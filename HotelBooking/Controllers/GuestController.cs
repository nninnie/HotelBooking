using HotelBooking.UI;

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
                string choice = CrudMenuUI.Show("Hantera gäster");

                switch (choice)
                {
                    case "Skapa ny":
                        _guestUI.Create(choice);
                        break;

                    case "Visa alla":
                        _guestUI.ShowAll(choice);
                        break;

                    case "Visa detaljer":
                        _guestUI.ShowDetails(choice);
                        break;

                    case "Uppdatera":
                        _guestUI.Update(choice);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
    }
}