using HotelBooking.Models;
using System;

namespace HotelBooking.Core
{
    public static class LoginService
    {
        public static Employee? CurrentEmployee { get; private set; }

        public static bool IsLoggedIn => CurrentEmployee != null;

        public static void Login(Employee employee)
        {
            CurrentEmployee = employee;
        }

        public static void Logout()
        {
            CurrentEmployee = null;
        }
    }
}