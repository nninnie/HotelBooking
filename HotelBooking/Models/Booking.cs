using System;
using System.Collections.Generic;

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

        // Saknas – måste finnas för EF Core
        public List<Invoice> Invoices { get; set; } = new();
    }
}