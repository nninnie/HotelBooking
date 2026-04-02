using HotelBooking.Models;
using HotelBooking.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.UI
{
    public class BookingMenu
    {
        private readonly BookingService _bookingService;

        public BookingMenu(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public void Show()
        {
            // 1. Hämta incheckning
            var checkIn = AskCheckInDate();

            // 2. Hämta utcheckning
            var checkOut = AskCheckOutDate(checkIn);

            // 3. Hämta antal gäster
            var guests = AskGuestCount();

            // 4. Hämta lediga rum
            var rooms = _bookingService.GetAvailableRooms(checkIn, checkOut, guests);

            // 5. Låt användaren välja rum
            var room = AskRoomSelection(rooms);

            // 6. Skapa bokning
            _bookingService.CreateBooking(room.Id, checkIn, checkOut, guests);

            // 7. Bekräftelse
            ShowConfirmation(room, checkIn, checkOut, guests);
        }

        private DateTime AskCheckInDate()
        {

        }

        private DateTime AskCheckOutDate(DateTime checkIn) 
        { 

        }
        private int AskGuestCount() 
        { 

        }
        private Room AskRoomSelection(List<Room> rooms) 
        { 

        }
        private void ShowConfirmation(Room room, DateTime checkIn, DateTime checkOut, int guests) 
        { 

        }
    }


}
}
