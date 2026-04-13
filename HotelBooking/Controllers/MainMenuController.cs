using HotelBooking.Core;
using HotelBooking.UI;

namespace HotelBooking.Controllers
{
    public class MainMenuController
    {
        private readonly BookingController _bookingController;
        private readonly RoomController _roomController;
        private readonly GuestController _guestController;
        private readonly PaymentController _paymentController;

        public MainMenuController(
            BookingController bookingController,
            RoomController roomController,
            GuestController guestController,
            PaymentController paymentController)
        {
            _bookingController = bookingController;
            _roomController = roomController;
            _guestController = guestController;
            _paymentController = paymentController;
        }

        public void Run()
        {
            while (true)
            {
                int choice = MainMenuUI.ShowMainMenu(LoginService.CurrentEmployee?.Username ?? "Okänt");

                switch (choice)
                {
                    case 1:
                        _bookingController.RunCrudMenu();
                        break;

                    case 2:
                        _roomController.RunCrudMenu();
                        break;

                    case 3:
                        _guestController.RunCrudMenu();
                        break;

                    case 4:
                        _paymentController.RunCrudMenu();
                        break;

                    case 5:
                        LoginService.CurrentEmployee?.Username = "Okänt";
                        return;

                    case 6:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}