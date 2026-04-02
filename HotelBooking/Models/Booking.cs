using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        // Relation till Room
        [Required]
        public int RoomId { get; set; }

        [ForeignKey(nameof(RoomId))]
        public Room? Room { get; set; }

        // Relation till Guest
        [Required]
        public int GuestId { get; set; }

        [ForeignKey(nameof(GuestId))]
        public Guest? Guest { get; set; }

        // Datumintervall
        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        // Saknas – måste finnas för EF Core
        public List<Invoice> Invoices { get; set; } = new();
    }
}