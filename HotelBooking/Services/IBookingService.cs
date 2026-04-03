using HotelBooking.Models;

namespace HotelBooking.Services
{
    // Interface för bokningslogik = DIP
    public interface IBookingService
    {
        // Hämta lediga rum baserat på datum och antal gäster
        List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests);

        // Skapa ny bokning
        Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int guests);

        // Hämta en specifik bokning
        Booking? GetBooking(int id);

        // Hämta alla bokningar
        List<Booking> GetAll();

        // Ta bort bokning permanent
        bool CancelBooking(int id);

        // Soft delete (markera som borttagen)
        bool SoftDelete(int id);

        // Återställ soft-deleted bokning
        bool Restore(int id);

        // Uppdatera bokning
        bool UpdateBooking(int id, int? guestId, DateTime? from, DateTime? to, int? guests);

        // Gäster
        List<Guest> GetAllGuests();
        Guest CreateGuest(string fullName, string email, string phone);
    }
}