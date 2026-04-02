using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        // Exempel: "101", "202", "B12"
        [Required]
        [MaxLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        // Exempel: "Single", "Double", "Suite"
        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;

        // Antal extrabäddar som kan läggas till
        [Range(0, 10)]
        public int ExtraBeds { get; set; }

        // Pris per natt
        [Required]
        [Range(0, 999999)]
        public decimal PricePerNight { get; set; }

        // Relation: ett rum kan ha många bokningar
        public List<Booking> Bookings { get; set; } = new();
    }
}