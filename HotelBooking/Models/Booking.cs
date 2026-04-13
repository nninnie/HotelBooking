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

    // Beräknad egenskap för att avgöra om bokningen är aktiv (inte raderad och inom datumintervallet)
    // [NotMapped] betyder att denna egenskap inte kommer att skapas som en kolumn i databasen
    [NotMapped]
    public bool IsActive
    {
        get
        {
            var today = DateTime.Today;
            return !IsDeleted && today >= FromDate && today <= ToDate;
        }
    }

    [NotMapped]
    public decimal TotalPrice
    {
        get
        {
            if (Room == null) return 0;

            int nights = (ToDate - FromDate).Days;
            if (nights < 1) nights = 1; // säkerhet om datumen är samma

            return nights * Room.PricePerNight;
        }
    }

    // Relation till Invoice
    public Invoice? Invoice { get; set; }
}