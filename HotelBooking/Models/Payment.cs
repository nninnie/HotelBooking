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

        // Betalningsinformation
        [Required]
        [Range(0, 999999)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        // Betalningsmetod (kan göras till enum om du vill)
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        // Status (valfritt)
        public bool IsSuccessful { get; set; }
    }
}