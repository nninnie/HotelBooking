using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.Services.Interfaces
{
    public interface ICrud
    {
        string MenuName { get; }
        void Run();
    }
}
