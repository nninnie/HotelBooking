namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        // Exempel: "101", "202", "B12"
        public string RoomNumber { get; set; } = string.Empty;

        // Exempel: "Single", "Double", "Suite"
        public string RoomType { get; set; } = string.Empty;

        // Antal extrabäddar som kan läggas till
        public int ExtraBeds { get; set; }

        // Pris per natt
        public decimal PricePerNight { get; set; }

        // Relation: ett rum kan ha många bokningar
        public List<Booking> Bookings { get; set; } = new();
    }
}