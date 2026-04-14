using Spectre.Console;

namespace HotelBooking.Core
{
    public static class ErrorService
    {
        private static void Print(string symbol, string color, string message)
        {
            AnsiConsole.MarkupLine($"\n[{color}]{symbol}  {message}[/]\n");
        }

        public static void Error(string message) =>
            Print("✖", "red", message);

        public static void Warning(string message) =>
            Print("⚠", "yellow", message);

        public static void Success(string message) =>
            Print("✔", "green", message);

        public static void Info(string message) =>
            Print("ℹ", "cyan", message);

        public static void FieldEmpty() =>
            Error("Fältet får inte vara tomt.");

        public static void InvalidNumber() =>
            Error("Du måste ange ett giltigt heltal.");

        public static void InvalidDate() =>
            Error("Ogiltigt datum! Ange formatet yyyy-MM-dd.");

        public static void NotFound(string item) =>
            Error($"{item} hittades inte.");

        public static void AlreadyExists(string item) =>
            Error($"{item} finns redan.");
    }
}
