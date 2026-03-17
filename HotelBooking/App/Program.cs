using HotelBooking.Controllers;
using HotelBooking.UI;

namespace HotelBooking.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            BannerUI.ShowLogo();

            MainMenuController.Start();
        }
    }
}
