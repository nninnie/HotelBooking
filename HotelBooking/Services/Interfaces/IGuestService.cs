using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IGuestService
    {
        IEnumerable<Guest> GetAllGuests();
        Guest? GetGuest(int id);
        Guest CreateGuest(string firstName, string lastName, DateTime birthDate, string? phone, string? email);
        bool UpdateGuest(int id, string? firstName, string? lastName, string? phone, string? email);
        bool DeleteGuest(int id);
        bool HasBookings(int guestId);
        IEnumerable<Guest> GetDeletedGuests();
        bool SoftDelete(int id);
        bool Restore(int id);

    }
}