using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IBookingService
    {
        List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests);
        Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int guests);
        Booking? GetBooking(int id);
        List<Booking> GetAll();
        bool CancelBooking(int id);
        bool SoftDelete(int id);
        bool Restore(int id);
        bool UpdateBooking(int id, int? guestId, DateTime? from, DateTime? to, int? guests);
        bool UpgradeBooking(int id);
        int GetActiveBookingCount(int guestId);
        int GetHistoryBookingCount(int guestId);
        decimal GetTotalSpentByGuest(int guestId);
        List<Booking> GetBookingsByGuestId(int guestId);
        IEnumerable<Booking> GetDeleted();
        bool CheckIn(int bookingId);
        bool CheckOut(int bookingId, bool payInvoice);


    }
}
