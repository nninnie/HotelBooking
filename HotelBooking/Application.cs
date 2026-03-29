using HotelBooking.Data;
using HotelBooking.UI;

namespace HotelBooking
{
    public class Application
    {
        private readonly BuildDB _buildDB;

        public Application(BuildDB buildDB)
        {
            _buildDB = buildDB;
        }

        public void Run()
        {
            _buildDB.Run(); // Skapar/uppdaterar databasen

            bool isLoggedIn = false;

            while (true)
            {
                var choice = MenuUI.ShowMainMenu(isLoggedIn);

                switch (choice)
                {
                    case 1:
                        // Logga in eller Ny bokning
                        break;

                    case 5:
                        // Avsluta eller Logga ut
                        return;
                }
            }
        }
    }
}