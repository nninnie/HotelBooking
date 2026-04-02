using HotelBooking.Controllers;
using HotelBooking.Data;

namespace HotelBooking
{
    public class Application
    {
        // BuildDB kör migrering + seed (startar databasen åt oss)
        private readonly BuildDB _buildDB;

        // Huvudmenyn som visas efter inloggning
        private readonly MainMenuController _mainMenuController;

        // Databascontext – behövs för login och menyer
        private readonly HotelDBContext _db;

        // DI: allt injiceras automatiskt från Program.cs
        public Application(BuildDB buildDB, MainMenuController mainMenuController, HotelDBContext db)
        {
            _buildDB = buildDB;
            _mainMenuController = mainMenuController;
            _db = db;
        }

        public void Run()
        {
            // Applikationen körs i en loop så man kan logga ut och in igen
            while (true)
            {
                bool isLoggedIn = false;

                // LoginController hanterar inloggning
                // Skickar in db + flaggan som styr om login lyckades
                LoginController.Login(_db, ref isLoggedIn);

                // Om login misslyckas → börja om och visa login igen
                if (!isLoggedIn)
                    continue;

                // När login lyckas - starta huvudmenyn
                _mainMenuController.Run();

                // När användaren loggar ut → loopen fortsätter och login visas igen
            }
        }
    }
}