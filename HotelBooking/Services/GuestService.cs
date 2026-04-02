using HotelBooking.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Services
{
    public class GuestService
    {
        private readonly HotelDBContext _db;

        public GuestService(HotelDBContext db)
        {
            _db = db;
        }

        public Guest[] GetAllGuests()
        {
            return _db.Guests.ToArray();

        }
    }
}
