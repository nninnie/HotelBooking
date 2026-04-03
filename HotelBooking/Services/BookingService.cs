using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class BookingService
    {
        private readonly HotelDBContext _db;

        public BookingService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämta lediga rum baserat på datum och antal gäster
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests)
        {
            // Datumvalidering
            if (checkIn.Date < DateTime.Today)
                throw new Exception("Incheckningsdatum kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckningsdatum måste vara efter incheckningsdatum.");

            // Hämta rum som klarar antal gäster, inte är ur bruk och inte är bokade
            return _db.Rooms
                .Where(r => r.MaxGuests >= guests && !r.IsOutOfService)
                .Where(r => !r.Bookings.Any(b =>
                    !b.IsDeleted &&
                    (
                        (checkIn >= b.FromDate && checkIn < b.ToDate) ||
                        (checkOut > b.FromDate && checkOut <= b.ToDate) ||
                        (checkIn <= b.FromDate && checkOut >= b.ToDate)
                    )
                ))
                .OrderBy(r => r.PricePerNight) // Sortera billigast först
                .ToList();
        }

        // Skapa ny bokning
        public Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
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

        // Hämta en specifik bokning
        public Booking? GetBooking(int id)
        {
            return _db.Bookings.FirstOrDefault(b => b.Id == id);
        }

        // Hämta alla bokningar
        public List<Booking> GetAll()
        {
            return _db.Bookings
                .OrderBy(b => b.FromDate)   // Sortera efter incheckning
                .ThenBy(b => b.RoomId)      // Sekundär sortering
                .ToList();
        }

        // Ta bort bokning permanent
        public bool CancelBooking(int id)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            _db.Bookings.Remove(booking);
            _db.SaveChanges();
            return true;
        }

        // Soft delete (markera som borttagen)
        public bool SoftDelete(int id)
        {
            var booking = GetBooking(id);
            if (booking == null || booking.IsDeleted) return false;

            booking.IsDeleted = true;
            _db.SaveChanges();
            return true;
        }

        // Återställ soft-deleted bokning
        public bool Restore(int id)
        {
            var booking = GetBooking(id);
            if (booking == null || !booking.IsDeleted) return false;

            booking.IsDeleted = false;
            _db.SaveChanges();
            return true;
        }

        // Uppdatera bokning
        public bool UpdateBooking(int id, int? guestId, DateTime? from, DateTime? to, int? guests)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            if (guestId.HasValue) booking.GuestId = guestId.Value;
            if (from.HasValue) booking.FromDate = from.Value;
            if (to.HasValue) booking.ToDate = to.Value;
            if (guests.HasValue) booking.NumberOfGuests = guests.Value;

            _db.SaveChanges();
            return true;
        }

        // Uppgradera bokning till dyrare rum
        public bool UpgradeBooking(int id)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            var currentRoom = _db.Rooms.Find(booking.RoomId);
            if (currentRoom == null) return false;

            var available = GetAvailableRooms(booking.FromDate, booking.ToDate, booking.NumberOfGuests);

            var upgraded = available.FirstOrDefault(r => r.PricePerNight > currentRoom.PricePerNight);
            if (upgraded == null) return false;

            booking.RoomId = upgraded.Id;
            _db.SaveChanges();
            return true;
        }

        // Hämta alla gäster (används i BookingUI)
        public List<Guest> GetAllGuests()
        {
            return _db.Guests
                .OrderBy(g => g.FullName) // Sortera alfabetiskt
                .ToList();
        }

        // Skapa ny gäst (används i BookingUI)
        public Guest CreateGuest(string fullName, string email, string phone)
        {
            var guest = new Guest
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phone
            };

            _db.Guests.Add(guest);
            _db.SaveChanges();

            return guest;
        }
    }
}