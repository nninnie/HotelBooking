using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.UI;

namespace HotelBooking
{
    public class MainMenuController
    {
        private readonly IEnumerable<ICrud> _crudOperations;
        private bool _isLoggedIn = false;

        public MainMenuController(IEnumerable<ICrud> crudOperations)
        {
            _crudOperations = crudOperations;
        }

        public void Run()
        {
            while (true)
            {
                int choice = MenuUI.ShowMainMenu(_isLoggedIn);

                switch (choice)
                {
                    case 1:
                        if (!_isLoggedIn)
                            _isLoggedIn = Login();
                        else
                            RunCrud("Ny bokning");
                        break;

                    case 2:
                        RunCrud(_isLoggedIn ? "Hantera rum" : "Visa rum");
                        break;

                    case 3:
                        RunCrud(_isLoggedIn ? "Hantera gäster" : "Visa gäster");
                        break;

                    case 4:
                        RunCrud(_isLoggedIn ? "Hantera bokningar" : "Visa bokningar");
                        break;

                    case 5:
                        if (_isLoggedIn)
                            _isLoggedIn = false;
                        else
                            return;
                        break;
                }
            }
        }

        private void RunCrud(string menuName)
        {
            var crud = _crudOperations.FirstOrDefault(c => c.MenuName == menuName);
            crud?.Run();
        }

        private bool Login()
        {
            // enkel inloggning
            return true;
        }
    }
}