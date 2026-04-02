using System;

namespace HotelBooking.Core.Service
{
    /// <summary>
    /// Lagrar namnet på den användare som för tillfället är inloggad.
    /// Används för att systemet alltid ska veta vem som använder hotellsystemet.
    /// </summary>
    public static class LoginService
    {
        private static string _currentUserName = "Okänd";

        public static string CurrentUserName
        {
            get => _currentUserName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _currentUserName = "Okänd";
                else
                    _currentUserName = value.Trim();
            }
        }

        /// <summary>
        /// Returnerar true om en användare är inloggad.
        /// </summary>
        public static bool IsLoggedIn =>
            _currentUserName != "Okänd";
    }
}