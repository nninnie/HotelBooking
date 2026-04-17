using HotelBooking.Core;
using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    // BookingService implementerar IBookingService
    public class BookingService : IBookingService
    {
        private readonly HotelDBContext _db;
        private readonly IInvoiceService _invoiceService;

        public BookingService(HotelDBContext db, IInvoiceService invoiceService)
        {
            _db = db;
            _invoiceService = invoiceService;
        }

        // Hämta alla rum som är lediga för ett visst datumintervall och antal gäster
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests)
        {
            if (checkIn.Date < DateTime.Today)
                throw new Exception("Incheckningsdatum kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckningsdatum måste vara efter incheckningsdatum.");

            // Hämta rum + bokningar
            var rooms = _db.Rooms
                .Include(r => r.Bookings)
                .Where(r => !r.IsOutOfService)
                .ToList();

            // Filtrera bort rum som har aktiva överlappande bokningar
            var available = rooms.Where(r =>
                !r.Bookings.Any(b =>
                    !b.IsDeleted &&
                    !b.IsCancelled &&
                    b.FromDate < checkOut &&
                    b.ToDate > checkIn
                )
            ); 

            // Filtrera på kapacitet
            return available
                .Where(r => (r.Capacity + r.ExtraBeds) >= guests)
                .OrderBy(r => r.PricePerNight)
                .ToList();
        }



        // Skapa en ny bokning
        public Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int guests)
        {
            if (checkIn < DateTime.Today)
                throw new Exception("Incheckning kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckning måste vara efter incheckning.");

            var booking = new Booking
            {
                GuestId = guestId,
                RoomId = roomId,
                FromDate = checkIn,
                ToDate = checkOut,
                NumberOfGuests = guests,
                BookingDate = DateTime.Now,
                CreatedByEmployeeId = LoginService.CurrentEmployee?.Id
                    ?? LoginService.GetFallbackEmployee(_db).Id
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();

            // Skapa faktura direkt
            _invoiceService.CreateForBooking(booking);

            return booking;
        }

        // Hämta en specifik bokning via ID
        public Booking? GetBooking(int id)
        {
            return _db.Bookings
                .Where(b => !b.IsDeleted && !b.IsCancelled)
                .Include(b => b.Guest)
                .Include(b => b.Room)
                .Include(b => b.Invoice)
                .FirstOrDefault(b => b.Id == id);
        }

        // Hämta alla bokningar sorterade efter datum och rum
        public List<Booking> GetAll()
        {
            return _db.Bookings
                .Where(b => !b.IsDeleted)                                                                                                                                                                                                                                   
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .Include(b => b.Invoice)
                .ToList();
        }


        // Ta bort bokning permanent (hard delete)
        public bool CancelBooking(int id)
        {
            var booking = _db.Bookings
                .Include(b => b.Invoice)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null) return false;

            // Ta bort faktura
            if (booking.Invoice != null)
                _db.Invoices.Remove(booking.Invoice);

            _db.Bookings.Remove(booking);
            _db.SaveChanges();
            return true;
        }

        public bool CancelDueToUnpaidInvoice(int bookingId)
        {
            var booking = _db.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null) return false;

            booking.IsCancelled = true;
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
            var booking = _db.Bookings
                .FirstOrDefault(b => b
                .Id == id && b
                .IsDeleted);
            if (booking == null) return false;

            booking.IsDeleted = false;
            _db.SaveChanges();
            return true;
        }
        // 
        public IEnumerable<Booking> GetDeleted()
        {
            return _db.Bookings
                .Where(b => b.IsDeleted)
                .ToList();
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

        // Hämta antal aktiva bokningar för en gäst
        public int GetActiveBookingCount(int guestId)
        {
            return _db.Bookings
                .Where(b => b.GuestId == guestId && !b.IsDeleted)
                .Count(b => b.IsActive);
        }
        // Hämta antal historiska (icke-aktiva) bokningar för en gäst
        public int GetHistoryBookingCount(int guestId)
        {
            return _db.Bookings
                .Where(b => b.GuestId == guestId && !b.IsDeleted)
                .Count(b => !b.IsActive);
        }
        // Hämta total spenderat belopp för en gäst (baserat på alla icke-raderade bokningar)
        public decimal GetTotalSpentByGuest(int guestId)
        {
            return _db.Bookings
                .Where(b => b.GuestId == guestId && !b.IsDeleted)
                .Sum(b => b.TotalPrice);
        }

        // Hämtar alla bokningar för en specifik gäst och sorterar dem efter startdatum
        public List<Booking> GetBookingsByGuestId(int guestId)
        {
            return _db.Bookings
                .Where(b => b.GuestId == guestId && !b.IsDeleted)
                .Include(b => b.Room)
                .Include(b => b.Guest)
                .OrderBy(b => b.FromDate)
                .ToList();
        }

        public bool CheckIn(int bookingId)
        {
            var booking = GetBooking(bookingId);
            if (booking == null)
                return false;

            // Tillåt check-in hela dagen
            if (DateTime.Today < booking.FromDate.Date)
                return false;

            booking.IsCheckedIn = true;
            booking.CheckInTime = DateTime.Now;

            _db.SaveChanges();
            return true;
        }

        public bool CheckOut(int bookingId, bool payInvoice)
        {
            var booking = GetBooking(bookingId);
            if (booking == null)
                return false;

            if (!booking.IsCheckedIn)
                return false;

            // Betala faktura om vald

            if (payInvoice && booking.Invoice != null)
            {
                booking.Invoice.IsPaid = true;
                booking.Invoice.PaidDate = DateTime.Now;
            }

            // Spara faktisk utcheckningstid
            booking.CheckOutTime = DateTime.Now;

            booking.IsCheckedIn = false;
            booking.IsCompleted = true;

            _db.SaveChanges();
            return true;
        }

    }
}