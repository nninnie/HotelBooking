using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotelBooking
{
    public class HotelDBContextFactory : IDesignTimeDbContextFactory<HotelDBContext>
    {
        public HotelDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HotelDBContext>();

            optionsBuilder.UseSqlServer(
                @"Server=.;Database=HotelBookingDB;Trusted_Connection=True;TrustServerCertificate=True;");

            return new HotelDBContext(optionsBuilder.Options);
        }
    }
}