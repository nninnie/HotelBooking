using Autofac;
using HotelBooking.Controllers;
using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services;
using HotelBooking.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HotelBooking
{
    public static class AutofacConfig
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // DbContext
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

            // Services (ren logik)
            builder.RegisterType<BookingService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RoomService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<GuestService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PaymentService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceService>().AsSelf().InstancePerLifetimeScope();   // <-- Lades till

            builder.RegisterType<DataInitializer>().AsSelf().InstancePerLifetimeScope();

            // UI-lager (Spectre.Console)
            builder.RegisterType<BookingUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RoomUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<GuestUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PaymentUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceUI>().AsSelf().InstancePerLifetimeScope();        // <-- Lades till

            // Controllers
            builder.RegisterType<BookingController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RoomController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<GuestController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PaymentController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceController>().AsSelf().InstancePerLifetimeScope(); // <-- Lades till
            builder.RegisterType<MainMenuController>().AsSelf().InstancePerLifetimeScope();

            // DB Builder
            builder.RegisterType<BuildDB>().AsSelf().InstancePerLifetimeScope();

            // Application
            builder.RegisterType<Application>().AsSelf().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}