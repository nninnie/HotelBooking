using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomType { get; set; } // Single / Double
        public int ExtraBeds { get; set; }
    }
}
