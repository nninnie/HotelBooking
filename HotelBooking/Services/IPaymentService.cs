using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IPaymentService
    {
        Payment CreatePayment(int invoiceId, decimal amount, string method);
        IEnumerable<Payment> GetPaymentsForInvoice(int invoiceId);
    }
}