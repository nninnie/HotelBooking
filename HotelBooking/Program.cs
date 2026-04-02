using Autofac;
using HotelBooking;
using HotelBooking.Data;

class Program
{
    static void Main(string[] args)
    {
        // 1. Bygg Autofac-container
        var container = AutofacConfig.BuildContainer();

        // 2. Skapa scope
        using var scope = container.BeginLifetimeScope();

        // 3. Kör DB-migrering + seed
        var dbBuilder = scope.Resolve<BuildDB>();
        dbBuilder.Run();

        // 4. Starta applikationen
        var app = scope.Resolve<Application>();
        app.Run();
    }
}