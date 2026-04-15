using HotelBooking.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    public int BookingId { get; set; }

    [ForeignKey(nameof(BookingId))]
    public Booking? Booking { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public DateTime? PaidDate { get; set; }

    [Required]
    [Range(0, 999999)]
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }
    public bool IsCancelled { get; set; } = false;
    public List<Payment> Payments { get; set; } = new();
}
