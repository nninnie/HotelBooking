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
            SeedEmployees(db);
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

        private void SeedEmployees(HotelDBContext db)
        {
            // 1. Seed roles om de saknas
            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new Role { Name = "Manager" },
                    new Role { Name = "Receptionist" },
                    new Role { Name = "Dungeon Master" }
                );

                db.SaveChanges(); // rollerna får Id
            }

            // 2. Seed employees om de saknas
            if (!db.Employees.Any())
            {
                // Hämta rollerna
                var manager = db.Roles.First(r => r.Name == "Manager");
                var receptionist = db.Roles.First(r => r.Name == "Receptionist");
                var master = db.Roles.First(r => r.Name == "Dungeon Master");

                // Enkel hash-funktion
                string Hash(string input) => input.GetHashCode().ToString();

                db.Employees.AddRange(
                    new Employee
                    {
                        Username = "ninnie",
                        FullName = "Ninnie Dahlström",
                        PasswordHash = Hash("0000"),
                        RoleId = manager.Id
                    },
                    new Employee
                    {
                        Username = "my",
                        FullName = "My Jammeh",
                        PasswordHash = Hash("1234"),
                        RoleId = receptionist.Id
                    },
                    new Employee
                    {
                        Username = "zoe",
                        FullName = "Zoe Jammeh",
                        PasswordHash = Hash("1234"),
                        RoleId = receptionist.Id
                    },
                    new Employee
                    {
                        Username = "richard",
                        FullName = "Richard Chalk",
                        PasswordHash = Hash("bajskorv"),
                        RoleId = master.Id
                    }
                );
            }
        }
    }
}