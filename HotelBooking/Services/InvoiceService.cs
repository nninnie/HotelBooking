using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly HotelDBContext _db;

        public InvoiceService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämta en specifik faktura
        public Invoice? GetInvoice(int id)
        {
            return _db.Invoices
                .FirstOrDefault(i => i.Id == id);
        }

        // Hämta alla fakturor
        public IEnumerable<Invoice> GetAllInvoices()
        {
            return _db.Invoices
                .OrderBy(i => i.Id)
                .ToList();
        }

        // Skapa en faktura kopplad till en bokning
        public Invoice CreateInvoice(int bookingId, int guestId, decimal totalAmount)
        {
            var invoice = new Invoice
            {
                BookingId = bookingId,
                GuestId = guestId,
                TotalAmount = totalAmount,
                CreatedDate = DateTime.Now,
                IsPaid = false
            };

            _db.Invoices.Add(invoice);
            _db.SaveChanges();

            return invoice;
        }

        // Markera faktura som betald
        public bool MarkAsPaid(int invoiceId)
        {
            var invoice = GetInvoice(invoiceId);
            if (invoice == null)
                return false;

            invoice.IsPaid = true;
            _db.SaveChanges();
            return true;
        }
    }
}