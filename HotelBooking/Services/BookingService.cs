using HotelBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelBooking.Services
{
    /// <summary>
    /// Provides booking-related operations for hotel rooms, including searching for available rooms, creating bookings,
    /// canceling bookings, and upgrading existing bookings.
    /// </summary>
    /// <remarks>This service acts as the main entry point for managing hotel room bookings and related
    /// actions. It requires a valid HotelDBContext instance for database access. All methods assume that the provided
    /// identifiers and parameters correspond to valid entities and values within the database context.</remarks>
    public class BookingService
    {
        private readonly HotelDBContext _db;

        public BookingService(HotelDBContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all rooms that are available between the given dates
        /// and can accommodate the specified number of guests.
        /// </summary>
        public List<Room> GetAvailableRooms(DateTime checkIn, DateTime checkOut, int guests)
        {
            // 1. Datumkontroll
            if (checkIn.Date < DateTime.Today)
                throw new Exception("Incheckningsdatum kan inte vara i det förflutna.");

            if (checkOut <= checkIn)
                throw new Exception("Utcheckningsdatum måste vara efter incheckningsdatum.");

            // 2. Hämta lediga rum
            var availableRooms = _db.Rooms
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

            return availableRooms;
        }

        /// <summary>
        /// Creates a new booking and stores NumberOfGuests + BookingDate.
        /// </summary>
        public Booking CreateBooking(int guestId, int roomId, DateTime checkIn, DateTime checkOut, int numberOfGuests)
        {
            var booking = new Booking
            {
                GuestId = guestId,
                RoomId = roomId,
                FromDate = checkIn,
                ToDate = checkOut,
                NumberOfGuests = numberOfGuests,
                BookingDate = DateTime.Now
            };

            _db.Bookings.Add(booking);
            _db.SaveChanges();
            return booking;
        }

        /// <summary>
        /// Cancels a booking by ID.
        /// </summary>
        public bool CancelBooking(int bookingId)
        {
            var booking = _db.Bookings
                .Find(bookingId);
            if (booking == null)
                return false;

            _db.Bookings.Remove(booking);
            _db.SaveChanges();
            return true;
        }

        /// <summary>
        /// Upgrades a booking to a more expensive available room.
        /// </summary>
        public bool UpgradeBooking(int bookingId)
        {
            // 1. Hitta bokningen
            var booking = _db
                .Bookings
                .Find(bookingId);
            if (booking == null)
                return false;

            var currentRoom = _db.Rooms
                .Find(booking
                .RoomId);
            if (currentRoom == null)
                return false;

            // 2. Hämta lediga rum baserat på antal gäster
            var availableRooms = GetAvailableRooms(
                booking.FromDate,
                booking.ToDate,
                booking.NumberOfGuests
            );

            // 3. Hitta första rummet som är dyrare
            var upgradedRoom = availableRooms
                .FirstOrDefault(r => r
                .PricePerNight > currentRoom
                .PricePerNight);

            if (upgradedRoom == null)
                return false;

            // 4. Uppgradera
            booking.RoomId = upgradedRoom.Id;
            _db.SaveChanges();

            return true;
        }

        public void Restore()
        {
 
        }

        public void SoftDelete()
        {

        }

        public void Update()
        {

        }

        public void Create()
        {
            
        }

        public void ShowDetails()
        {
            
        }

        public void ShowAll()
        {
            
        }
    }
}