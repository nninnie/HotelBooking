using HotelBooking.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    public int Id { get; set; }

    public int CreatedByEmployeeId { get; set; }
    public Employee? CreatedByEmployee { get; set; }

    [Required]
    public int RoomId { get; set; }
    public Room? Room { get; set; }

    [Required]
    public int GuestId { get; set; }
    public Guest? Guest { get; set; }

    public int NumberOfGuests { get; set; }

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    public DateTime BookingDate { get; set; } = DateTime.Now;

    public bool IsDeleted { get; set; } = false;     // Soft delete
    public bool IsCancelled { get; set; } = false;   // Avbokad / obetald

    [NotMapped]
    public bool IsActive
    {
        get
        {
            var today = DateTime.Today;
            return !IsDeleted && !IsCancelled && today >= FromDate && today <= ToDate;
        }
    }

    [NotMapped]
    public decimal TotalPrice
    {
        get
        {
            if (Room == null) return 0;

            int nights = (ToDate - FromDate).Days;
            if (nights < 1) nights = 1;

            return nights * Room.PricePerNight;
        }
    }

    public Invoice? Invoice { get; set; }
}