using System;

namespace HotelBooking.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // Relation till Invoice
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        // Betalningsinformation
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        // Betalningsmetod (kan göras till enum om du vill)
        public string? Method { get; set; }

        // Status (valfritt)
        public bool IsSuccessful { get; set; }
    }
}