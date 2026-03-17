using System;

namespace HotelBooking.Core.Service
{
    /// <summary>
    /// Stores the name of the currently logged-in user.
    /// Used so the system always knows who is operating the hotel system.
    /// </summary>
    public static class LoginService
    {
        private static string _currentUserName = "Unknown";

        public static string CurrentUserName
        {
            get => _currentUserName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _currentUserName = "Unknown";
                else
                    _currentUserName = value.Trim();
            }
        }

        /// <summary>
        /// Returns true if a user is logged in.
        /// </summary>
        public static bool IsLoggedIn =>
            _currentUserName != "Unknown";
    }
}