using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Staybnb.Models;
using System.Reflection.Emit;

namespace Staybnb.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<HostProperty> HostProperties { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<HostApplication> HostApplications { get; set; }
        public DbSet<CheckInProcess> CheckInProcesses { get; set; }
        public DbSet<GuestCheckIn> GuestCheckIns { get; set; }
        public DbSet<GuestDocument> GuestDocuments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Property → Host (One-to-Many, Restrict delete)
            builder.Entity<HostProperty>()
                .HasOne(p => p.Host)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking → Property (Restrict)
            builder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking → Guest (Restrict)
            builder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment → Booking (One-to-One)
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId);

            // GuestCheckIn → Booking (One-to-One)
            builder.Entity<GuestCheckIn>()
                .HasOne(g => g.Booking)
                .WithOne(b => b.GuestCheckIn)
                .HasForeignKey<GuestCheckIn>(g => g.BookingId);

            // CheckInProcess → Property (One-to-One)
            builder.Entity<CheckInProcess>()
                .HasOne(c => c.Property)
                .WithOne(p => p.CheckInProcess)
                .HasForeignKey<CheckInProcess>(c => c.PropertyId);

            // Property ↔ Amenity (Many-to-Many)
            builder.Entity<HostProperty>()
                .HasMany(p => p.Amenities)
                .WithMany(a => a.Properties)
                .UsingEntity(j => j.ToTable("PropertyAmenities"));
        }
    }
}