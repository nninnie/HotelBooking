using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class GuestService : IGuestService
    {
        private readonly HotelDBContext _db;

        public GuestService(HotelDBContext db)
        {
            _db = db;
        }

        public IEnumerable<Guest> GetAllGuests()
        {
            return _db.Guests
                .OrderBy(g => g.FullName)
                .ToList();
        }
        public Guest? GetGuest(int id)
        {
            return _db.Guests.FirstOrDefault(g => g.Id == id);
        }

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
    }
}