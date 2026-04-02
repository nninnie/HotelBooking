using System;

namespace HotelBooking.Core.Service
{
    public static class ErrorService
    {
        private static void PrintMessage(string symbol, ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{symbol}  {message}\n");
            Console.ResetColor();
        }

        // Generiska meddelanden
        public static void Show(string message) =>
            PrintMessage("✖", ConsoleColor.Red, message);

        public static void Warning(string message) =>
            PrintMessage("!", ConsoleColor.Yellow, message);

        public static void Success(string message) =>
            PrintMessage("✔", ConsoleColor.Green, message);

        // Fördefinierade felmeddelanden
        public static void FieldEmpty() =>
            Show("Fältet får inte vara tomt.");

        public static void InvalidNumber() =>
            Show("Vänligen ange ett giltigt nummer.");

        public static void InvalidDate() =>
            Show("Vänligen ange ett giltigt datum (ÅÅÅÅ-MM-DD).");

        public static void NotFound(string itemName) =>
            Show($"{itemName} hittades inte.");

        public static void AlreadyExists(string itemName) =>
            Show($"{itemName} finns redan.");

        public static void Unauthorized() =>
            Show("Du måste vara inloggad för att utföra denna åtgärd.");
    }
}