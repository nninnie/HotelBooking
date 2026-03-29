namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        // Typ av rum, t.ex. "Single", "Double", "Suite"
        public string RoomType { get; set; }

        // Antal extrabäddar som kan läggas till
        public int ExtraBeds { get; set; }

        // Pris per natt
        public decimal PricePerNight { get; set; }

        // Relation: ett rum kan ha många bokningar över tid
        public List<Booking> Bookings { get; set; } = new();
    }
}