using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class BuildDB
    {
        private readonly DataInitializer _initializer;

        public BuildDB(DataInitializer initializer)
        {
            _initializer = initializer;
        }

        public void Run()
        {
            _initializer.MigrateAndSeed();
        }
    }
}
