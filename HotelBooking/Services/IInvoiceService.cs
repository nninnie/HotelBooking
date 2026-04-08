using HotelBooking.Models;

namespace HotelBooking.Services
{
    public interface IInvoiceService
    {
        Invoice CreateInvoice(int bookingId);
        Invoice? GetInvoice(int id);
        IEnumerable<Invoice> GetAllInvoices();
    }
}