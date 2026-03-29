using System.Collections.Generic;

namespace HotelBooking.Models
{
    public class Guest
    {
        public int Id { get; set; }                 // Primärnyckel
        public string FullName { get; set; }        // Gästens namn
        public string PhoneNumber { get; set; }     // Telefonnummer
        public string Email { get; set; }           // E-postadress

        // Relation: En gäst kan ha flera bokningar
        public List<Booking> Bookings { get; set; } = new();
    }
}