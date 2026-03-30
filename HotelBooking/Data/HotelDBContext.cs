using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking
{
    // Arv från DbContext
    public class HotelDBContext : DbContext
    {
        // Konstruktorn som hämtar base(options)
        // Tar emot databasinställningar och skickar dem vidare till DbContext‑basen.
        public HotelDBContext(DbContextOptions<HotelDBContext> options)
            : base(options)
        {
        }

        // Alla modellklasser byggs upp i OnModelCreating
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Guest → Booking (många bokningar per gäst)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guest → Invoice (många fakturor per gäst)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Guest)
                .WithMany(g => g.Invoices)
                .HasForeignKey(i => i.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking → Invoice (en bokning kan ha flera fakturor)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Booking)
                .WithMany(b => b.Invoices)
                .HasForeignKey(i => i.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room → Booking (ett rum kan ha många bokningar)
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Room)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice → Payment (en faktura kan ha flera betalningar)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}