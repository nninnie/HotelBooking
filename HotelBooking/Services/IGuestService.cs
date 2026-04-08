using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IGuestService
    {
        IEnumerable<Guest> GetAllGuests();
        Guest? GetGuest(int id);
        Guest CreateGuest(string fullName, string? phone, string? email);
        bool UpdateGuest(int id, string? fullName, string? phone, string? email);
    }
}