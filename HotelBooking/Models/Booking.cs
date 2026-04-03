using HotelBooking.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    public int Id { get; set; }

    // Relation till Room
    [Required]
    public int RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public Room? Room { get; set; }

    // Relation till Guest
    [Required]
    public int GuestId { get; set; }

    [ForeignKey(nameof(GuestId))]
    public Guest? Guest { get; set; }

    // Antal gäster i bokningen
    public int NumberOfGuests { get; set; }

    // Datumintervall
    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    // Datum då bokningen skapades (viktigt för betalningsdeadline)
    public DateTime BookingDate { get; set; } = DateTime.Now;
    
    public bool IsDeleted { get; set; } = false;

    // Relation till Invoice
    public List<Invoice> Invoices { get; set; } = new();
}