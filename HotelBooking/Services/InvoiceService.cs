using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly HotelDBContext _db;

        public InvoiceService(HotelDBContext db)
        {
            _db = db;
        }

        // Skapa faktura automatiskt för en bokning
        public Invoice CreateForBooking(Booking booking)
        {
            var invoice = new Invoice
            {
                BookingId = booking.Id,
                CreatedDate = DateTime.Now,
                TotalAmount = booking.TotalPrice,
                DueDate = booking.BookingDate.AddDays(10),
                IsPaid = false
            };

            _db.Invoices.Add(invoice);
            _db.SaveChanges();
            return invoice;
        }
        public IEnumerable<Invoice> GetAllInvoices()
        {
            return _db.Invoices
                .Include(i => i.Booking!)
                    .ThenInclude(b => b.Guest!)
                .Include(i => i.Booking!.Room!)
                .ToList();
        }

        public Invoice? GetInvoice(int id)
        {
            return _db.Invoices
                .Include(i => i.Booking!)
                    .ThenInclude(b => b.Guest!)
                .Include(i => i.Booking!.Room!)
                .FirstOrDefault(i => i.Id == id);
        }


        public bool MarkAsPaid(int id)
        {
            var invoice = _db.Invoices.FirstOrDefault(i => i.Id == id);
            if (invoice == null) return false;

            invoice.IsPaid = true;
            invoice.PaidDate = DateTime.Now;

            _db.SaveChanges();
            return true;
        }
    }
}
