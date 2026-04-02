using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class BuildDB
    {
        private readonly HotelDBContext _context;
        private readonly DataInitializer _initializer;

        public BuildDB(HotelDBContext context, DataInitializer initializer)
        {
            _context = context;
            _initializer = initializer;
        }

        public void Run()
        {
            _initializer.MigrateAndSeed(_context);
        }
    }
}