using System;

namespace HotelBooking.Core
{
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

        public static bool IsLoggedIn =>
            _currentUserName != "Okänd";
    }
}