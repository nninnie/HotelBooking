using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IInvoiceService
    {
        Invoice CreateInvoice(int bookingId, int guestId, decimal totalAmount);
        Invoice? GetInvoice(int id);
        IEnumerable<Invoice> GetAllInvoices();
        bool MarkAsPaid(int invoiceId);
    }
}