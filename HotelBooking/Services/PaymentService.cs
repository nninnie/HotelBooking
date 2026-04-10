using HotelBooking.Data;
using HotelBooking.Models;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HotelDBContext _db;

        public PaymentService(HotelDBContext db)
        {
            _db = db;
        }

        // Hämta betalning kopplad till en faktura
        public IEnumerable<Payment> GetPaymentsForInvoice(int invoiceId)
        {
            return _db.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .ToList();
        }

        // Skapa en betalning
        public Payment CreatePayment(int invoiceId, decimal amount)
        {
            var payment = new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentDate = DateTime.Now,
                IsSuccessful = true
            };

            _db.Payments.Add(payment);
            _db.SaveChanges();

            return payment;
        }

        // Lista alla betalningar
        public List<Payment> GetAllPayments()
        {
            return _db.Payments
                .OrderBy(p => p.Id)
                .ToList();
        }

        // Hämta en specifik betalning
        public Payment? GetPayment(int id)
        {
            return _db.Payments.FirstOrDefault(p => p.Id == id);
        }


    }
}