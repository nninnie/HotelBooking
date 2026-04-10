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
                    new Room { RoomNumber = "101", RoomType = "Enkelrum", Capacity = 1, ExtraBeds = 0, PricePerNight = 900m },
                    new Room { RoomNumber = "102", RoomType = "Enkelrum", Capacity = 1, ExtraBeds = 1, PricePerNight = 1050m },
                    new Room { RoomNumber = "103", RoomType = "Enkelrum", Capacity = 1, ExtraBeds = 0, PricePerNight = 900m },
                    new Room { RoomNumber = "104", RoomType = "Enkelrum", Capacity = 1, ExtraBeds = 1, PricePerNight = 1050m },
                    new Room { RoomNumber = "201", RoomType = "Dubbelrum", Capacity = 2, ExtraBeds = 1, PricePerNight = 1200m },
                    new Room { RoomNumber = "202", RoomType = "Dubbelrum", Capacity = 2, ExtraBeds = 1, PricePerNight = 1200m },
                    new Room { RoomNumber = "203", RoomType = "Dubbelrum", Capacity = 2, ExtraBeds = 2, PricePerNight = 1400m },
                    new Room { RoomNumber = "204", RoomType = "Dubbelrum", Capacity = 2, ExtraBeds = 2, PricePerNight = 1400m }
                );
            }
        }

        private void SeedGuests(HotelDBContext db)
        {
            if (!db.Guests.Any())
            {
                db.Guests.AddRange(
                    new Guest { FirstName = "Anna", LastName = "Svensson", PhoneNumber = "0701234567", Email = "anna@example.com" },
                    new Guest { FirstName = "Markus", LastName = "Lindgren", PhoneNumber = "0709876543", Email = "markus@example.com" },
                    new Guest { FirstName = "Sara", LastName = "Ekström", PhoneNumber = "0735551122", Email = "sara@example.com" },
                    new Guest { FirstName = "David", LastName = "Holm", PhoneNumber = "0764442211", Email = "david@example.com" }
                );
            }
        }

        private void SeedEmployees(HotelDBContext db)
        {
            // Seed roles
            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new Role { Name = "Manager" },
                    new Role { Name = "Receptionist" },
                    new Role { Name = "Dungeon Master" }
                );

                db.SaveChanges(); // rollerna får Id
            }

            // Seed employees 
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
                        FirstName = "Ninnie",     
                        LastName = "Dahlström",   
                        PasswordHash = Hash("1234"),
                        RoleId = manager.Id
                    },
                    new Employee
                    {
                        Username = "my",
                        FirstName = "My",         
                        LastName = "Jammeh",      
                        PasswordHash = Hash("1234"),
                        RoleId = receptionist.Id
                    },
                    new Employee
                    {
                        Username = "zoe",
                        FirstName = "Zoe",       
                        LastName = "Jammeh",      
                        PasswordHash = Hash("1234"),
                        RoleId = receptionist.Id
                    },
                    new Employee
                    {
                        Username = "richard",
                        FirstName = "Richard",   
                        LastName = "Chalk",      
                        PasswordHash = Hash("bajskorv"),
                        RoleId = master.Id
                    }
                );
            }
        }
    }
}