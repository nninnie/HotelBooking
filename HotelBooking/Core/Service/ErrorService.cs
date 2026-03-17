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

        // Generic
        public static void Show(string message) =>
            PrintMessage("✖", ConsoleColor.Red, message);

        public static void Warning(string message) =>
            PrintMessage("!", ConsoleColor.Yellow, message);

        public static void Success(string message) =>
            PrintMessage("✔", ConsoleColor.Green, message);

        // Predefined errors
        public static void FieldEmpty() =>
            Show("The field cannot be empty.");

        public static void InvalidNumber() =>
            Show("Please enter a valid number.");

        public static void InvalidDate() =>
            Show("Please enter a valid date (YYYY-MM-DD).");

        public static void NotFound(string itemName) =>
            Show($"{itemName} was not found.");

        public static void AlreadyExists(string itemName) =>
            Show($"{itemName} already exists.");

        public static void Unauthorized() =>
            Show("You must be logged in to perform this action.");
    }
}