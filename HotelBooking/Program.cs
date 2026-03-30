using HotelBooking.Controllers;
using HotelBooking.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Data;

namespace HotelBooking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Läs in appsettings.json (hämtar connection string)
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // 2. Bygg konfigurationen
            var config = builder.Build();
            var connectionString = config.GetConnectionString("DefaultConnection");

            // 3. Registrera tjänster i DI-containern
            var services = new ServiceCollection();

            // Registrerar DbContext med SQL Server-anslutningen
            services.AddDbContext<HotelDBContext>(options =>
                options.UseSqlServer(connectionString));

            // Registrerar klasser som ska kunna skapas via DI
            services.AddTransient<BuildDB>();
            services.AddTransient<Application>();

            using var provider = services.BuildServiceProvider();

            // 4. Kör migrationer (skapar/uppdaterar databasen)
            var dbBuilder = provider.GetRequiredService<BuildDB>();
            dbBuilder.Run();

            // 5. Kör seedning (lägger in startdata om tabellerna är tomma)
            using (var scope = provider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<HotelDBContext>();
                new DataInitializer().MigrateAndSeed(db);
            }

            // 6. Starta applikationen
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BannerUI.ShowLogo();

            var app = provider.GetRequiredService<Application>();
            app.Run();
        }
    }
}