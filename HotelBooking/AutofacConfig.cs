using Autofac;
using HotelBooking.Controllers;
using HotelBooking.Data;
using HotelBooking.Services;
using HotelBooking.Services.Interfaces;
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

            // Services (registrera som interface!)
            builder.RegisterType<InvoiceService>().As<IInvoiceService>().InstancePerLifetimeScope();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<BookingService>().As<IBookingService>().InstancePerLifetimeScope();
            builder.RegisterType<GuestService>().As<IGuestService>().InstancePerLifetimeScope();
            builder.RegisterType<RoomService>().As<IRoomService>().InstancePerLifetimeScope();

            // UI-lager
            builder.RegisterType<BookingUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RoomUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<GuestUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PaymentUI>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceUI>().AsSelf().InstancePerLifetimeScope();

            // Controllers
            builder.RegisterType<BookingController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RoomController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<GuestController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<PaymentController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<InvoiceController>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MainMenuController>().AsSelf().InstancePerLifetimeScope();

            // DataInitializer & DB Builder
            builder.RegisterType<DataInitializer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<BuildDB>().AsSelf().InstancePerLifetimeScope();

            // Application
            builder.RegisterType<Application>().AsSelf().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}