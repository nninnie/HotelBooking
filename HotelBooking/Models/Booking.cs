using System;

namespace HotelBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Relation till Room
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        // Relation till Guest
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }

        // Datumintervall
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}