using HotelBooking.Models;
using HotelBooking.Data;

namespace HotelBooking.Core
{
    public static class LoginService
    {
        public static Employee? CurrentEmployee { get; private set; }

        public static void Login(Employee employee)
        {
            CurrentEmployee = employee;
        }

        public static void Logout()
        {
            CurrentEmployee = null;
        }

        // Hämta fallback från databasen
        public static Employee GetFallbackEmployee(HotelDBContext db)
        {
            return db.Employees.First(e => e.Username == "TestUser");
        }
    }
}