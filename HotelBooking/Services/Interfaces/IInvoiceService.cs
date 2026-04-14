using HotelBooking.Models;

namespace HotelBooking.Services.Interfaces
{
    public interface IInvoiceService
    {
        Invoice CreateForBooking(Booking booking);
        Invoice? GetInvoice(int id);
        IEnumerable<Invoice> GetAllInvoices();
        bool MarkAsPaid(int invoiceId);
        int CancelUnpaidBookings();
        void DeleteInvoiceForBooking(int bookingId);
    }
}