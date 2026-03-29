using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class BuildDB
    {
        private readonly HotelDBContext _context;

        public BuildDB(HotelDBContext context)
        {
            _context = context;
        }

        public void Run()
        {
            _context.Database.Migrate();
        }
    }
}