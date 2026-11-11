using HotelBooking.Controllers;
using HotelBooking.Core;
using HotelBooking.Data;
using HotelBooking.Services.Interfaces;

namespace HotelBooking
{
    public class Application
    {
        // Huvudmenyn som visas efter inloggning
        private readonly MainMenuController _mainMenuController;

        // Databascontext – behövs för login och menyer
        private readonly HotelDBContext _db;

        // Bokningslogik – används för att annullera obetalda bokningar vid start
        private readonly IBookingService _bookingService;

        // DI: allt injiceras automatiskt från Program.cs (databasen byggs i Program.cs innan Run)
        public Application(MainMenuController mainMenuController, HotelDBContext db, IBookingService bookingService)
        {
            _mainMenuController = mainMenuController;
            _db = db;
            _bookingService = bookingService;
        }

        public void Run()
        {
            // Bokningar med obetald faktura efter förfallodatum annulleras automatiskt
            _bookingService.CancelOverdueUnpaidBookings();

            // Applikationen körs i en loop så man kan logga ut och in igen
            while (true)
            {
                bool isLoggedIn = false;

                // LoginController hanterar inloggning
                // Skickar in db + flaggan som styr om login lyckades
                LoginController.Login(_db, ref isLoggedIn);

                // Om login misslyckas - börja om och visa login igen
                if (!isLoggedIn)
                    continue;

                // När login lyckas - starta huvudmenyn
                _mainMenuController.Run();

                // När användaren loggar ut - nollställ inloggad anställd, loopen fortsätter och login visas igen
                LoginService.Logout();
            }
        }
    }
}