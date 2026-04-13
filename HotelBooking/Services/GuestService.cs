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

        // Hämta alla gäster sorterade efter efternamn + förnamn
        public IEnumerable<Guest> GetAllGuests()
        {
            return _db.Guests
                .OrderBy(g => g.LastName)   
                .ThenBy(g => g.FirstName)
                .ToList();
        }

        // Hämta en gäst baserat på ID
        public Guest? GetGuest(int id)
        {
            return _db.Guests
                .FirstOrDefault(g => g.Id == id);
        }

        // Skapa en ny gäst
        public Guest CreateGuest(string firstName, string lastName, string? phone, string? email)
        {
            var guest = new Guest
            {
                FirstName = firstName,   
                LastName = lastName,     
                PhoneNumber = phone,
                Email = email
            };

            _db.Guests.Add(guest);
            _db.SaveChanges();
            return guest;
        }

        // Uppdatera en gästs information
        public bool UpdateGuest(int id, string? firstName, string? lastName, string? phone, string? email)
        {
            var guest = GetGuest(id);
            if (guest == null)
                return false;

            // uppdatera endast om värden finns
            if (!string.IsNullOrWhiteSpace(firstName)) guest.FirstName = firstName;
            if (!string.IsNullOrWhiteSpace(lastName)) guest.LastName = lastName;
            if (!string.IsNullOrWhiteSpace(phone)) guest.PhoneNumber = phone;
            if (!string.IsNullOrWhiteSpace(email)) guest.Email = email;

            _db.SaveChanges();
            return true;
        }

        // Kontrollera om gästen har bokningar
        public bool HasBookings(int guestId)
        {
            return _db.Bookings.Any(b => b.GuestId == guestId && !b.IsDeleted);
        }

        // Ta bort en gäst
        public bool DeleteGuest(int id)
        {
            var guest = _db.Guests
                .FirstOrDefault(g => g.Id == id);
            if (guest == null)
                return false;

            _db.Guests.Remove(guest);
            _db.SaveChanges();
            return true;
        }

        public IEnumerable<Guest> GetDeletedGuests()
        {
            return _db.Guests
                .Where(g => g.IsDeleted)
                .OrderBy(g => g.LastName)
                .ThenBy(g => g.FirstName)
                .ToList();
        }

        public bool SoftDelete(int id)
        {
            var guest = _db.Guests.FirstOrDefault(g => g.Id == id);
            if (guest == null)
                return false;

            guest.IsDeleted = true;
            _db.SaveChanges();
            return true;
        }

        public bool Restore(int id)
        {
            var guest = _db.Guests.FirstOrDefault(g => g.Id == id);
            if (guest == null)
                return false;

            guest.IsDeleted = false;
            _db.SaveChanges();
            return true;
        }
    }
}