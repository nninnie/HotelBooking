using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using HotelBooking.Services;
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
                int choice = MainMenuUI.ShowMainMenu(_isLoggedIn);

                switch (choice)
                {
                    case 1:
                            _isLoggedIn = Login();
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
            if (_isLoggedIn)
                return true;

            var username = AnsiConsole.Ask<string>("Användarnamn:");
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("Lösenord:")
                .PromptStyle("red")
                .Secret()
                );

            if (username == "a" && password == "a")
            {
                _isLoggedIn = true;
                AnsiConsole.MarkupLine("[green]Inloggning lyckades![/]");
                Console.ReadKey();
                return true;
            }
            AnsiConsole.MarkupLine("[red]Fel användarnamn eller lösenord.[/]");
            return false;
        }
    }
}