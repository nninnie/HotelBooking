using System;

namespace HotelBooking.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        // Relation till Booking
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        // Relation till Guest
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }

        // Fakturadetaljer
        public DateTime CreatedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}