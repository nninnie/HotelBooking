using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    // BookingService implementerar IBookingService (SOLID: DIP)
    public class BookingService : IBookingService
    {
        private readonly HotelDBContext _db;

        public BookingService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämta alla rum som är lediga för ett visst datumintervall och antal gäster
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests)
        {
            // Grundläggande datumvalidering
            if (checkIn.Date < DateTime.Today)
                throw new Exception("Incheckningsdatum kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckningsdatum måste vara efter incheckningsdatum.");

            // Filtrera rum:
            // 1. Rummet måste klara antal gäster
            // 2. Rummet får inte vara ur bruk
            // 3. Rummet får inte ha en aktiv bokning som överlappar datumintervallet
            return _db.Rooms
                .Where(r => (r.Capacity + r.ExtraBeds) >= guests && !r.IsOutOfService)
                .Where(r => !r.Bookings.Any(b =>
                    !b.IsDeleted &&
                    (
                        (checkIn >= b.FromDate && checkIn < b.ToDate) ||
                        (checkOut > b.FromDate && checkOut <= b.ToDate) ||
                        (checkIn <= b.FromDate && checkOut >= b.ToDate)
                    )
                ))
                .OrderBy(r => r.PricePerNight) // Visa billigaste först
                .ToList();
        }

        // Skapa en ny bokning
        public Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            // Datumvalidering
            if (checkIn < DateTime.Today)
                throw new Exception("Incheckning kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckning måste vara efter incheckning.");

            // Skapa bokningsobjektet
            var booking = new Booking
            {
                GuestId = guestId,
                RoomId = roomId,
                FromDate = checkIn,
                ToDate = checkOut,
                NumberOfGuests = guests,
                BookingDate = DateTime.Now,
                IsDeleted = false
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();
            return booking;
        }

        // Hämta en specifik bokning via ID
        public Booking? GetBooking(int id)
        {
            return _db.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == id);
        }

        // Hämta alla bokningar sorterade efter datum och rum
        public List<Booking> GetAll()
        {
            return _db.Bookings
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .ToList();
        }

        // Ta bort bokning permanent (hard delete)
        public bool CancelBooking(int id)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            _db.Bookings.Remove(booking);
            _db.SaveChanges();
            return true;
        }

        // Soft delete – markera bokningen som borttagen utan att radera den
        public bool SoftDelete(int id)
        {
            var booking = GetBooking(id);
            if (booking == null || booking.IsDeleted) return false;

            booking.IsDeleted = true;
            _db.SaveChanges();
            return true;
        }

        // Återställ en soft-deleted bokning
        public bool Restore(int id)
        {
            var booking = GetBooking(id);
            if (booking == null || !booking.IsDeleted) return false;

            booking.IsDeleted = false;
            _db.SaveChanges();
            return true;
        }

        // Uppdatera valfria delar av en bokning
        public bool UpdateBooking(int id, int? guestId, DateTime? from, DateTime? to, int? guests)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            // Endast de värden som skickas in uppdateras
            if (guestId.HasValue) booking.GuestId = guestId.Value;
            if (from.HasValue) booking.FromDate = from.Value;
            if (to.HasValue) booking.ToDate = to.Value;
            if (guests.HasValue) booking.NumberOfGuests = guests.Value;

            _db.SaveChanges();
            return true;
        }

        // Försök uppgradera till ett dyrare rum som är ledigt
        public bool UpgradeBooking(int id)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            var currentRoom = _db.Rooms.Find(booking.RoomId);
            if (currentRoom == null) return false;

            // Hämta alla lediga rum för samma datumintervall
            var available = GetAvailableRooms(booking.FromDate, booking.ToDate, booking.NumberOfGuests);

            // Hitta första rummet som är dyrare än nuvarande
            var upgraded = available.FirstOrDefault(r => r.PricePerNight > currentRoom.PricePerNight);
            if (upgraded == null) return false;

            booking.RoomId = upgraded.Id;
            _db.SaveChanges();
            return true;
        }
    }
}