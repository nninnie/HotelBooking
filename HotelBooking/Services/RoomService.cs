using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class RoomService
    {
        private readonly HotelDBContext _db;

        public RoomService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämta alla rum
        public List<Room> GetAllRooms()
        {
            return _db.Rooms
                .OrderBy(r => r.Id)
                .ToList();
        }

        // Hämta ett specifikt rum
        public Room? GetRoom(int id)
        {
            return _db.Rooms
                .FirstOrDefault(r => r.Id == id);
        }

        // Skapa nytt rum
        public Room CreateRoom(string roomNumber, string roomType, int baseGuests, int extraBeds, decimal price)
        {
            var room = new Room
            {
                RoomNumber = roomNumber,
                RoomType = roomType,
                BaseGuests = baseGuests,
                ExtraBeds = extraBeds,
                PricePerNight = price,
                IsOutOfService = false
            };

            _db.Rooms.Add(room);
            _db.SaveChanges();

            return room;
        }

        // Uppdatera rum
        public bool UpdateRoom(int id, string roomNumber, string roomType, int baseGuests, int extraBeds, decimal price, bool isOutOfService)
        {
            var room = GetRoom(id);
            if (room == null)
                return false;

            room.RoomNumber = roomNumber;
            room.RoomType = roomType;
            room.BaseGuests = baseGuests;
            room.ExtraBeds = extraBeds;
            room.PricePerNight = price;
            room.IsOutOfService = isOutOfService;

            _db.SaveChanges();
            return true;
        }

        // Soft delete (sätt OutOfService = true)
        public bool SoftDeleteRoom(int id)
        {
            var room = GetRoom(id);
            if (room == null)
                return false;

            room.IsOutOfService = true;
            _db.SaveChanges();
            return true;
        }

        // Restore (sätt OutOfService = false)
        public bool RestoreRoom(int id)
        {
            var room = GetRoom(id);
            if (room == null)
                return false;

            room.IsOutOfService = false;
            _db.SaveChanges();
            return true;
        }
    }
}