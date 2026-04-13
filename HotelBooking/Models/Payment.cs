using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Models
{
    public class Payment
    {
        public int Id { get; set; }

        // Relation till Invoice
        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public Invoice? Invoice { get; set; }

        [Required]
        public PaymentMethod Method { get; set; } = PaymentMethod.Faktura;

        [Required]
        [Range(0, 999999)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        public bool IsSuccessful { get; set; }

    }
}