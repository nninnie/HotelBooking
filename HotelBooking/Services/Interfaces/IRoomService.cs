using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IRoomService
    {
        IEnumerable<Room> GetAllRooms();
        IEnumerable<Room> GetActiveRooms();
        IEnumerable<Room> GetDeletedRooms();

        Room? GetRoom(int id);

        Room CreateRoom(
    string roomNumber,
    string roomType,
    int squareMeters,
    int capacity,
    int extraBeds,
    decimal pricePerNight,
    bool isOutOfService,
    bool hasSeaView,
    bool hasMinibar,
    bool isPetFriendly,
    bool hasBalcony,
    bool hasBathtub
);

        bool UpdateRoom(
            int id,
            string number,
            string type,
            int baseGuests,
            int extraBeds,
            decimal price,
            bool outOfService,
            bool hasSeaView,
            bool hasMinibar,
            bool isPetFriendly,
            bool hasBalcony,
            bool hasBathtub
        );

        bool SoftDelete(int id);
        bool Restore(int id);
    }
}