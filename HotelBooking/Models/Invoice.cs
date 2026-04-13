using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [ForeignKey(nameof(BookingId))]
        public Booking? Booking { get; set; }

        [Required]
        public int GuestId { get; set; }

        [ForeignKey(nameof(GuestId))]
        public Guest? Guest { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Range(0, 999999)]
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; }

        public List<Payment> Payments { get; set; } = new();
    }
}