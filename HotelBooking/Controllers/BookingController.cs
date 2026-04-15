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
                // CrudMenuUI 
                string? choice = CrudMenuUI.Show(
    "Hantera bokningar",
    "Välj ett alternativ",
    "Skapa ny",
    "Checka in",
    "Checka ut",
    "Visa alla",
    "Visa detaljer",
    "Uppdatera",
    "Ta bort",
    "Återställ",
    "Tillbaka"
);


                if (choice == null)
                    return; 

                switch (choice)
                {
                    case "Skapa ny":
                        _bookingUI.Create(choice);  
                        break;

                    case "Checka in":
                        _bookingUI.CheckIn(choice);
                        break;

                    case "Checka ut":
                        _bookingUI.CheckOut(choice);
                        break;

                    case "Visa alla":
                        _bookingUI.ShowAll(choice);  
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