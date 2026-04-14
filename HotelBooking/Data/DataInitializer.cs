using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class DataInitializer
    {
        private readonly HotelDBContext _db;
        private readonly IInvoiceService _invoiceService;

        public DataInitializer(HotelDBContext db, IInvoiceService invoiceService)
        {
            _db = db;
            _invoiceService = invoiceService;
        }

        public void MigrateAndSeed()
        {
            _db.Database.Migrate();

            SeedRooms();
            _db.SaveChanges();

            SeedGuests();
            _db.SaveChanges();

            SeedEmployees();
            _db.SaveChanges();

            SeedBooking();
            _db.SaveChanges();
        }

        private void SeedRooms()
        {
            if (_db.Rooms.Any())
                return;

            _db.Rooms.AddRange(
                new Room { RoomNumber = "101", RoomType = "Enkelrum", SquareMeters = 15, Capacity = 1, ExtraBeds = 0, PricePerNight = 850m, HasSeaView = false, HasMinibar = false, IsPetFriendly = false, HasBalcony = false, HasBathtub = false },
                new Room { RoomNumber = "102", RoomType = "Enkelrum", SquareMeters = 20, Capacity = 1, ExtraBeds = 1, PricePerNight = 950m, HasSeaView = false, HasMinibar = true, IsPetFriendly = false, HasBalcony = false, HasBathtub = false },
                new Room { RoomNumber = "103", RoomType = "Enkelrum", SquareMeters = 15, Capacity = 1, ExtraBeds = 0, PricePerNight = 900m, HasSeaView = true, HasMinibar = false, IsPetFriendly = false, HasBalcony = true, HasBathtub = false },

                new Room { RoomNumber = "201", RoomType = "Dubbelrum", SquareMeters = 30, Capacity = 2, ExtraBeds = 1, PricePerNight = 1200m, HasSeaView = false, HasMinibar = true, IsPetFriendly = true, HasBalcony = false, HasBathtub = false },
                new Room { RoomNumber = "202", RoomType = "Dubbelrum", SquareMeters = 30, Capacity = 2, ExtraBeds = 1, PricePerNight = 1300m, HasSeaView = true, HasMinibar = true, IsPetFriendly = false, HasBalcony = true, HasBathtub = false },
                new Room { RoomNumber = "203", RoomType = "Dubbelrum", SquareMeters = 30, Capacity = 2, ExtraBeds = 2, PricePerNight = 1450m, HasSeaView = true, HasMinibar = true, IsPetFriendly = true, HasBalcony = true, HasBathtub = true },

                new Room { RoomNumber = "301", RoomType = "Familjerum", SquareMeters = 40, Capacity = 3, ExtraBeds = 2, PricePerNight = 1800m, HasSeaView = false, HasMinibar = true, IsPetFriendly = true, HasBalcony = false, HasBathtub = true },
                new Room { RoomNumber = "302", RoomType = "Familjerum", SquareMeters = 50, Capacity = 4, ExtraBeds = 2, PricePerNight = 1950m, HasSeaView = true, HasMinibar = true, IsPetFriendly = false, HasBalcony = true, HasBathtub = true },

                new Room { RoomNumber = "401", RoomType = "Svit", SquareMeters = 60, Capacity = 3, ExtraBeds = 2, PricePerNight = 2600m, HasSeaView = true, HasMinibar = true, IsPetFriendly = false, HasBalcony = true, HasBathtub = true },
                new Room { RoomNumber = "402", RoomType = "Svit", SquareMeters = 60, Capacity = 3, ExtraBeds = 2, PricePerNight = 2800m, HasSeaView = true, HasMinibar = true, IsPetFriendly = true, HasBalcony = true, HasBathtub = true }
            );
        }

        private void SeedGuests()
        {
            if (_db.Guests.Any())
                return;

            _db.Guests.AddRange(
                new Guest { FirstName = "Anna", LastName = "Svensson", BirthDate = new DateTime(1982, 2, 3), PhoneNumber = "0701234567", Email = "anna@example.com" },
                new Guest { FirstName = "Markus", LastName = "Lindgren", BirthDate = new DateTime(1975, 5, 8), PhoneNumber = "0709876543", Email = "markus@example.com" },
                new Guest { FirstName = "Sara", LastName = "Ekström", BirthDate = new DateTime(1982, 8, 15), PhoneNumber = "0735551122", Email = "sara@example.com" },
                new Guest { FirstName = "David", LastName = "Holm", BirthDate = new DateTime(1991, 9, 26), PhoneNumber = "0764442211", Email = "david@example.com" }
            );
        }

        private void SeedEmployees()
        {
            if (!_db.Roles.Any())
            {
                _db.Roles.AddRange(
                    new Role { Name = "Manager" },
                    new Role { Name = "Receptionist" },
                    new Role { Name = "Dungeon Master" }
                );
                _db.SaveChanges();
            }

            if (_db.Employees.Any())
                return;

            var manager = _db.Roles.First(r => r.Name == "Manager");
            var receptionist = _db.Roles.First(r => r.Name == "Receptionist");
            var master = _db.Roles.First(r => r.Name == "Dungeon Master");

            string Hash(string input) => input.GetHashCode().ToString();

            _db.Employees.AddRange(
                new Employee { Username = "ninnie", FirstName = "Ninnie", LastName = "Dahlström", PasswordHash = Hash("1234"), RoleId = manager.Id },
                new Employee { Username = "my", FirstName = "My", LastName = "Jammeh", PasswordHash = Hash("1234"), RoleId = master.Id },
                new Employee { Username = "zoe", FirstName = "Zoe", LastName = "Jammeh", PasswordHash = Hash("1234"), RoleId = master.Id },
                new Employee { Username = "richard", FirstName = "Richard", LastName = "Chalk", PasswordHash = Hash("bajskorv"), RoleId = master.Id },
                new Employee { Username = "TestUser", FirstName = "Test", LastName = "User", PasswordHash = "", RoleId = receptionist.Id }
            );
        }

        private void SeedBooking()
        {
            if (_db.Bookings.Any())
                return;

            var booking = new Booking
            {
                GuestId = _db.Guests.First().Id,
                RoomId = _db.Rooms.First().Id,
                CreatedByEmployeeId = _db.Employees.First().Id,
                FromDate = DateTime.Today.AddDays(1),
                ToDate = DateTime.Today.AddDays(5),
                NumberOfGuests = 1,
                BookingDate = DateTime.Now
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();

            _invoiceService.CreateForBooking(booking);
        }
    }
}
