using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class BuildDB
    {
        private readonly HotelContext _context;

        public BuildDB(HotelContext context)
        {
            _context = context;
        }

        public void Run()
        {
            _context.Database.Migrate();
        }
    }
}