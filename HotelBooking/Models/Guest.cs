using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class Guest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        // En gäst kan ha flera bokningar
        public List<Booking> Bookings { get; set; } = new();

        // En gäst kan ha flera fakturor 
        public List<Invoice> Invoices { get; set; } = new();
    }
}


