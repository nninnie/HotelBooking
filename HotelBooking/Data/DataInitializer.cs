using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class DataInitializer
    {
        public void MigrateAndSeed(HotelDBContext db)
        {
            db.Database.Migrate();
            SeedRooms(db);
            SeedGuests(db);
            db.SaveChanges();
        }

        private void SeedRooms(HotelDBContext db)
        {
            if (!db.Rooms.Any())
            {
                db.Rooms.AddRange(
                    new Room { RoomNumber = "101", RoomType = "Single", BaseGuests = 1, ExtraBeds = 0, PricePerNight = 850m },
                    new Room { RoomNumber = "102", RoomType = "Double", BaseGuests = 2, ExtraBeds = 1, PricePerNight = 1200m },
                    new Room { RoomNumber = "201", RoomType = "Double", BaseGuests = 2, ExtraBeds = 2, PricePerNight = 1400m },
                    new Room { RoomNumber = "202", RoomType = "Single", BaseGuests = 1, ExtraBeds = 0, PricePerNight = 900m }
                );
            }
        }

        private void SeedGuests(HotelDBContext db)
        {
            if (!db.Guests.Any())
            {
                db.Guests.AddRange(
                    new Guest { FullName = "Anna Svensson", PhoneNumber = "0701234567", Email = "anna@example.com" },
                    new Guest { FullName = "Markus Lindgren", PhoneNumber = "0709876543", Email = "markus@example.com" },
                    new Guest { FullName = "Sara Ekström", PhoneNumber = "0735551122", Email = "sara@example.com" },
                    new Guest { FullName = "David Holm", PhoneNumber = "0764442211", Email = "david@example.com" }
                );
            }
        }
    }
}