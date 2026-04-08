using HotelBooking.Core.Service;
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
                int choice = MainMenuUI.ShowMainMenu(LoginService.CurrentUserName);

                switch (choice)
                {
                    case 1:
                        // Hantera bokningar
                        _bookingController.RunCrudMenu();
                        break;

                    case 2:
                        // Hantera rum
                        _roomController.RunCrudMenu();
                        break;

                    case 3:
                        // Hantera gäster
                        _guestController.RunCrudMenu();
                        break;

                    case 4:
                        // Hantera betalningar
                        _paymentController.RunCrudMenu();
                        break;

                    case 5:
                        // Logga ut
                        LoginService.CurrentUserName = "Okänd";
                        return;

                    case 6:
                        // Avsluta
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}