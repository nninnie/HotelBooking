using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IPaymentService
    {
        Payment CreatePayment(int invoiceId, decimal amount);
        IEnumerable<Payment> GetPaymentsForInvoice(int invoiceId);
        List<Payment> GetAllPayments();
        Payment? GetPayment(int id);
    }
}