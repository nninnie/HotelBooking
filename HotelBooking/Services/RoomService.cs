using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class RoomService : IRoomService
    {
        private readonly HotelDBContext _db;

        public RoomService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämtar alla rum (både aktiva och ur funktion)
        public IEnumerable<Room> GetAllRooms()
        {
            return _db.Rooms.ToList();
        }

        // Hämtar endast aktiva rum
        public IEnumerable<Room> GetActiveRooms()
        {
            return _db.Rooms.Where(r => !r.IsOutOfService).ToList();
        }

        // Hämtar endast rum som är markerade som ur funktion (soft deleted)
        public IEnumerable<Room> GetDeletedRooms()
        {
            return _db.Rooms.Where(r => r.IsOutOfService).ToList();
        }

        public Room? GetRoom(int id)
        {
            return _db.Rooms.FirstOrDefault(r => r.Id == id);
        }

        public Room CreateRoom(string number, string type, int capacity, int extraBeds, decimal price, bool outOfService)
        {
            var room = new Room
            {
                RoomNumber = number,
                RoomType = type,
                Capacity = capacity,
                ExtraBeds = extraBeds,
                PricePerNight = price,
                IsOutOfService = outOfService
            };

            _db.Rooms.Add(room);
            _db.SaveChanges();
            return room;
        }

        public bool UpdateRoom(int id, string number, string type, int capacity, int extraBeds, decimal price, bool outOfService)
        {
            var room = GetRoom(id);
            if (room == null) return false;

            room.RoomNumber = number;
            room.RoomType = type;
            room.Capacity = capacity;
            room.ExtraBeds = extraBeds;
            room.PricePerNight = price;
            room.IsOutOfService = outOfService;

            _db.SaveChanges();
            return true;
        }

        // Soft delete = markera som ur funktion
        public bool SoftDelete(int id)
        {
            var room = GetRoom(id);
            if (room == null) return false;

            room.IsOutOfService = true;
            _db.SaveChanges();
            return true;
        }

        // Återställ soft deleted rum
        public bool Restore(int id)
        {
            var room = GetRoom(id);
            if (room == null) return false;

            room.IsOutOfService = false;
            _db.SaveChanges();
            return true;
        }
    }
}