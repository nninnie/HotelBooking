using HotelBooking.Models;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class GuestService : IGuestService
    {
        private readonly HotelDBContext _db;

        public GuestService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämta alla gäster sorterade efter namn
        public IEnumerable<Guest> GetAllGuests()
        {
            return _db.Guests
                .OrderBy(g => g.FullName)
                .ToList();
        }

        // Hämta en gäst baserat på ID
        public Guest? GetGuest(int id)
        {
            return _db.Guests.FirstOrDefault(g => g.Id == id);
        }

        // Skapa en ny gäst
        public Guest CreateGuest(string fullName, string? phone, string? email)
        {
            var guest = new Guest
            {
                FullName = fullName,
                PhoneNumber = phone,
                Email = email
            };

            _db.Guests.Add(guest);
            _db.SaveChanges();
            return guest;
        }

        // Uppdatera en gästs information
        public bool UpdateGuest(int id, string? fullName, string? phone, string? email)
        {
            var guest = GetGuest(id);
            if (guest == null)
                return false;

            if (!string.IsNullOrWhiteSpace(fullName)) guest.FullName = fullName;
            if (!string.IsNullOrWhiteSpace(phone)) guest.PhoneNumber = phone;
            if (!string.IsNullOrWhiteSpace(email)) guest.Email = email;

            _db.SaveChanges();
            return true;
        }

        // Kontrollera om gästen har bokningar
        public bool HasBookings(int guestId)
        {
            return _db.Bookings.Any(b => b.GuestId == guestId);
        }

        // Ta bort en gäst
        public bool DeleteGuest(int id)
        {
            var guest = _db.Guests.FirstOrDefault(g => g.Id == id);
            if (guest == null)
                return false;

            _db.Guests.Remove(guest);
            _db.SaveChanges();
            return true;
        }
    }
}