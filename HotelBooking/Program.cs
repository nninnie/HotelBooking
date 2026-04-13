using Autofac;
using HotelBooking;
using HotelBooking.Data;

class Program
{
    static void Main(string[] args)
    {
        // Ställ in UTF-8 som standardkodning för konsolen
        Console.OutputEncoding = System.Text.Encoding.UTF8;

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