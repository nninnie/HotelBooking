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
    public bool IsCheckedIn { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsDeleted { get; set; } = false;   
    public bool IsCancelled { get; set; } = false;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    [NotMapped]
    public bool IsActive
    {
        get
        {
            var today = DateTime.Today;
            return !IsDeleted && !IsCancelled &&
                   today >= FromDate.Date &&
                   today <= ToDate.Date;
        }
    }


    [NotMapped]
    public int Nights => Math.Max(1, (ToDate.Date - FromDate.Date).Days);

    [NotMapped]
    public decimal TotalPrice => Room?.PricePerNight is decimal price
    ? Nights * price
    : 0;

    public Invoice? Invoice { get; set; }
}