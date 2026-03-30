using System.Collections.Generic;

namespace HotelBooking.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        // En gäst kan ha flera bokningar
        public List<Booking> Bookings { get; set; } = new();

        // En gäst kan ha flera fakturor (saknades)
        public List<Invoice> Invoices { get; set; } = new();
    }
}