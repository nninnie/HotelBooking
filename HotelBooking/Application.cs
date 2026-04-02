using HotelBooking.Controllers;
using HotelBooking.Data;

namespace HotelBooking
{
    public class Application
    {
        // BuildDB ansvarar för migrering + seedning (körs i Program.cs)
        private readonly BuildDB _buildDB;

        // MainMenuController hanterar huvudmenyn efter inloggning
        private readonly MainMenuController _mainMenuController;

        // Dependency Injection: Application får sina beroenden automatiskt
        public Application(BuildDB buildDB, MainMenuController mainMenuController)
        {
            _buildDB = buildDB;
            _mainMenuController = mainMenuController;
        }

        public void Run()
        {
            // Oändlig loop som håller applikationen igång
            while (true)
            {
                bool isLoggedIn = false;

                // Kör login-processen
                // LoginController sätter isLoggedIn = true om användaren loggar in korrekt
                LoginController.Login(ref isLoggedIn);

                // Om inloggningen misslyckas → börja om från början av loopen
                if (!isLoggedIn)
                    continue;

                // När användaren är inloggad → starta huvudmenyn
                _mainMenuController.Run();

                // När användaren loggar ut → loopen börjar om och visar login igen
            }
        }
    }
}