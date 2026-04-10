using Autofac;
using HotelBooking;
using HotelBooking.Data;

class Program
{
    static void Main(string[] args)
    {
        // Bygg Autofac-container
        var container = AutofacConfig.BuildContainer();

        // Skapa scope
        using var scope = container.BeginLifetimeScope();

        // Kör DB-migrering + seed
        var dbBuilder = scope.Resolve<BuildDB>();
        dbBuilder.Run();

        // Starta applikationen
        var app = scope.Resolve<Application>();
        app.Run();
    }
}