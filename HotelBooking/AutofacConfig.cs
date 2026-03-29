using Autofac;
using HotelBooking.Data;
using System.ComponentModel;
using IContainer = Autofac.IContainer;

namespace HotelBooking
{
    public static class AutofacConfig
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // CRUD – Booking
            builder.RegisterType<CreateBooking>().As<ICrud>();
            builder.RegisterType<ReadBooking>().As<ICrud>();
            builder.RegisterType<UpdateBooking>().As<ICrud>();
            builder.RegisterType<DeleteBooking>().As<ICrud>();

            // CRUD – Room
            builder.RegisterType<CreateRoom>().As<ICrud>();

            // CRUD – Customer
            builder.RegisterType<CreateCustomer>().As<ICrud>();

            // Database setup (migrations + seed)
            builder.RegisterType<BuildDB>().AsSelf();

            // Meny
            builder.RegisterType<MainMenu>().AsSelf();

            // Application
            builder.RegisterType<Application>().AsSelf();

            return builder.Build();
        }
    }
}