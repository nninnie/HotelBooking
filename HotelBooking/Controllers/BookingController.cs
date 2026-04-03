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

        public void CreateDirect()
        {
            _bookingUI.Create(); // “Ny bokning” från huvudmenyn
        }

        public void RunCrudMenu()
        {
            while (true)
            {
                int choice = CrudMenuUI.Show("Hantera bokningar");

                switch (choice)
                {
                    case 1:
                        _bookingUI.ShowAll();
                        break;

                    case 2:
                        _bookingUI.ShowDetails();
                        break;

                    case 3:
                        _bookingUI.Create();
                        break;

                    case 4:
                        _bookingUI.Update();
                        break;

                    case 5:
                        _bookingUI.SoftDelete();
                        break;

                    case 6:
                        _bookingUI.Restore();
                        break;

                    case 7:
                        return; // Tillbaka till huvudmenyn
                }
            }
        }
    }
}