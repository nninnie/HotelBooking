using Autofac;
using HotelBooking.Controllers;
using HotelBooking.Data;
using HotelBooking.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelBooking
{
    public static class AutofacConfig
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // Läs appsettings.json
            builder.Register(c =>
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var options = new DbContextOptionsBuilder<HotelDBContext>()
                    .UseSqlServer(config.GetConnectionString("DefaultConnection"))
                    .Options;

                return new HotelDBContext(options);
            })
            .AsSelf()
            .InstancePerLifetimeScope();

            // Services
            builder.RegisterType<BookingService>().AsSelf().SingleInstance();
            builder.RegisterType<RoomService>().AsSelf().SingleInstance();
            builder.RegisterType<GuestService>().AsSelf().SingleInstance();
            builder.RegisterType<PaymentService>().AsSelf().SingleInstance();

            // Controllers
            builder.RegisterType<BookingController>().AsSelf();
            builder.RegisterType<RoomController>().AsSelf();
            builder.RegisterType<GuestController>().AsSelf();
            builder.RegisterType<PaymentController>().AsSelf();
            builder.RegisterType<MainMenuController>().AsSelf();

            // DB Builder
            builder.RegisterType<BuildDB>().AsSelf();

            // Application
            builder.RegisterType<Application>().AsSelf();

            return builder.Build();
        }
    }
}