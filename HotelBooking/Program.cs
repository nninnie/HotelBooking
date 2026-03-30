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
            // 1. Läs in appsettings.json
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();
            var connectionString = config.GetConnectionString("DefaultConnection");

            // 2. Dependency Injection container
            var services = new ServiceCollection();

            services.AddDbContext<HotelDBContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddTransient<BuildDB>();
            services.AddTransient<Application>();

            using var provider = services.BuildServiceProvider();

            // 3. Kör migrationer vid uppstart
            var dbBuilder = provider.GetRequiredService<BuildDB>();
            dbBuilder.Run();

            // 4. Starta applikationen
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BannerUI.ShowLogo();

            var app = provider.GetRequiredService<Application>();
            app.Run();
        }
    }
}