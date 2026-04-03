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

        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests)
        {
            if (checkIn.Date < DateTime.Today)
                throw new Exception("Incheckningsdatum kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckningsdatum måste vara efter incheckningsdatum.");

            return _db.Rooms
                .Where(r => r.MaxGuests >= guests)
                .Where(r => !_db.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    (
                        (checkIn >= b.FromDate && checkIn < b.ToDate) ||
                        (checkOut > b.FromDate && checkOut <= b.ToDate) ||
                        (checkIn <= b.FromDate && checkOut >= b.ToDate)
                    )
                ))
                .OrderBy(r => r.PricePerNight)
                .ToList();
        }

        public Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            var booking = new Booking
            {
                GuestId = guestId,
                RoomId = roomId,
                FromDate = checkIn,
                ToDate = checkOut,
                NumberOfGuests = guests,
                BookingDate = DateTime.Now
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();
            return booking;
        }

        public Booking? GetBooking(int id)
        {
            return _db.Bookings.FirstOrDefault(b => b.Id == id);
        }

        public List<Booking> GetAll()
        {
            return _db.Bookings.OrderBy(b => b.Id).ToList();
        }

        public bool CancelBooking(int id)
        {
            var booking = GetBooking(id);
            if (booking == null) return false;

            _db.Bookings.Remove(booking);
            _db.SaveChanges();
            return true;
        }

        public bool SoftDelete(int id)
        {
            var booking = GetBooking(id);
            if (booking == null || booking.IsDeleted) return false;

            booking.IsDeleted = true;
            _db.SaveChanges();
            return true;
        }

        public bool Restore(int id)
        {
            var booking = GetBooking(id);
            if (booking == null || !booking.IsDeleted) return false;

            booking.IsDeleted = false;
            _db.SaveChanges();
            return true;
        }

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

        public List<Guest> GetAllGuests()
        {
            return _db.Guests.OrderBy(g => g.FullName).ToList();
        }

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

        public List<Room> GetAvailableRooms(DateTime from, DateTime to)
        {
            return _db.Rooms
                .Where(r => !r.Bookings.Any(b =>
                    (from < b.ToDate && to > b.FromDate) && !b.IsDeleted))
                .ToList();
        }
    }
}