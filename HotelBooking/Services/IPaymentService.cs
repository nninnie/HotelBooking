using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IPaymentService
    {
        Payment CreatePayment(int bookingId, decimal amount, string method);
        IEnumerable<Payment> GetPaymentsForBooking(int bookingId);
    }
}