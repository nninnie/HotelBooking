using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            return _db.Rooms
                .OrderBy(r => r.RoomNumber)
                .ToList();
        }

        // Hämtar endast aktiva rum
        public IEnumerable<Room> GetActiveRooms()
        {
            return _db.Rooms
                .Where(r => !r.IsOutOfService)
                .ToList();
        }

        // Hämtar endast rum som är markerade som ur funktion (soft deleted)
        public IEnumerable<Room> GetDeletedRooms()
        {
            return _db.Rooms
                .Where(r => r.IsOutOfService)
                .ToList();
        }

        public Room? GetRoom(int id)
        {
            return _db.Rooms
                .FirstOrDefault(r => r.Id == id);
        }

        // SKAPA RUM – nu med alla nya features
        public Room CreateRoom(
            string number,
            string type,
            int capacity,
            int extraBeds,
            decimal price,
            bool outOfService,
            bool hasSeaView,
            bool hasMinibar,
            bool isPetFriendly,
            bool hasBalcony,
            bool hasBathtub)
        {
            var room = new Room
            {
                RoomNumber = number,
                RoomType = type,
                Capacity = capacity,
                ExtraBeds = extraBeds,
                PricePerNight = price,
                IsOutOfService = outOfService,

                // Nya features
                HasSeaView = hasSeaView,
                HasMinibar = hasMinibar,
                IsPetFriendly = isPetFriendly,
                HasBalcony = hasBalcony,
                HasBathtub = hasBathtub
            };

            _db.Rooms.Add(room);
            _db.SaveChanges();
            return room;
        }

        // UPPDATERA RUM – nu med alla nya features
        public bool UpdateRoom(
            int id,
            string number,
            string type,
            int capacity,
            int extraBeds,
            decimal price,
            bool outOfService,
            bool hasSeaView,
            bool hasMinibar,
            bool isPetFriendly,
            bool hasBalcony,
            bool hasBathtub)
        {
            var room = GetRoom(id);
            if (room == null) return false;

            room.RoomNumber = number;
            room.RoomType = type;
            room.Capacity = capacity;
            room.ExtraBeds = extraBeds;
            room.PricePerNight = price;
            room.IsOutOfService = outOfService;
            room.HasSeaView = hasSeaView;
            room.HasMinibar = hasMinibar;
            room.IsPetFriendly = isPetFriendly;
            room.HasBalcony = hasBalcony;
            room.HasBathtub = hasBathtub;

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