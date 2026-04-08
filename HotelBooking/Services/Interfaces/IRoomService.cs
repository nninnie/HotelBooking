using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IRoomService
    {
        IEnumerable<Room> GetAllRooms();
        Room? GetRoom(int id);
        Room CreateRoom(string number, string type, int baseGuests, int extraBeds, decimal price, bool outOfService);
        bool UpdateRoom(int id, string number, string type, int baseGuests, int extraBeds, decimal price, bool outOfService);
        bool SoftDelete(int id);
        bool Restore(int id);
    }
}