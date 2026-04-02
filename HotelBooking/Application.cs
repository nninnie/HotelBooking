using HotelBooking.Controllers;
using HotelBooking.Data;

namespace HotelBooking
{
    public class Application
    {
        private readonly BuildDB _buildDB;
        private readonly MainMenuController _mainMenuController;

        public Application(BuildDB buildDB, MainMenuController mainMenuController)
        {
            _buildDB = buildDB;
            _mainMenuController = mainMenuController;
        }

        public void Run()
        {
            // Kör migrations + seed
            _buildDB.Run();

            while (true)
            {
                bool isLoggedIn = false;

                // Login innan huvudmenyn
                LoginController.Login(ref isLoggedIn);

                if (!isLoggedIn)
                    continue;

                // Starta huvudmenyn
                _mainMenuController.Run();

                // Om användaren loggar ut → loopar tillbaka till login
            }
        }
    }
}