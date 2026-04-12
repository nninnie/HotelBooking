using HotelBooking.Services;
using HotelBooking.UI;

namespace HotelBooking.Controllers
{
    public class BookingController
    {
        private readonly BookingUI _bookingUI;

        public BookingController(BookingUI bookingUI)
        {
            _bookingUI = bookingUI;
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                // CrudMenuUI returnerar nu TEXT, inte int
                string choice = CrudMenuUI.Show("Hantera bokningar");

                switch (choice)
                {
                    case "Skapa ny":
                        _bookingUI.Create(choice);   // choice = "Skapa ny"
                        break;

                    case "Visa alla":
                        _bookingUI.ShowAll(choice);  // choice = "Visa alla"
                        break;

                    case "Visa detaljer":
                        _bookingUI.ShowDetails(choice);
                        break;

                    case "Uppdatera":
                        _bookingUI.Update(choice);
                        break;

                    case "Ta bort":
                        _bookingUI.SoftDelete(choice);
                        break;

                    case "Återställ":
                        _bookingUI.Restore(choice);
                        break;

                    case "Tillbaka":
                        return;
                }
            }
        }
    }
}