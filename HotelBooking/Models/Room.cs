using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; } = string.Empty;


        [Range(1, 10)]
        public int Capacity { get; set; }

        [Range(0, 2)]
        public int ExtraBeds { get; set; }

        public int MaxGuests => Capacity + ExtraBeds;

        [Required]
        [Range(0, 999999)]
        public decimal PricePerNight { get; set; }
        public bool IsOutOfService { get; set; } = false;

        public List<Booking> Bookings { get; set; } = new();
    }
}