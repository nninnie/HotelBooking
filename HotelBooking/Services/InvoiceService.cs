using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;

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
        // Ta bort obetalda bokningar som är äldre än 10 dagar  
        public int CancelUnpaidBookings()
        {
            var cutoff = DateTime.Now.AddDays(-10);

            var overdueInvoices = _db.Invoices
                .Where(i => !i.IsPaid && i.CreatedDate < cutoff)
                .ToList();

            int count = 0;

            foreach (var invoice in overdueInvoices)
            {
                var booking = _db.Bookings.FirstOrDefault(b => b.Id == invoice.BookingId);

                if (booking != null && !booking.IsDeleted)
                {
                    booking.IsDeleted = true;   // Soft delete
                    count++;
                }

                _db.Invoices.Remove(invoice);   //  Fakturan tas bort
            }

            _db.SaveChanges();
            return count;
        }
    }
}