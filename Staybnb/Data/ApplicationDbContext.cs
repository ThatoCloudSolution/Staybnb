using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Staybnb.Models;

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

            // ===== Message — two FKs to ApplicationUser cause cascade path cycle =====
            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Booking — two FKs (Property + Guest) =====
            builder.Entity<Booking>()
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Guest)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== HostProperty → Host =====
            builder.Entity<HostProperty>()
                .HasOne(p => p.Host)
                .WithMany(u => u.Properties)
                .HasForeignKey(p => p.HostId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Review — two FKs =====
            builder.Entity<Review>()
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== WishlistItem — two FKs =====
            builder.Entity<WishlistItem>()
                .HasOne(w => w.User)
                .WithMany(u => u.WishlistItems)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WishlistItem>()
                .HasOne(w => w.Property)
                .WithMany()
                .HasForeignKey(w => w.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Notification → User =====
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== ActivityLog → User =====
            builder.Entity<ActivityLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== HostApplication — two FKs =====
            builder.Entity<HostApplication>()
                .HasOne(a => a.ApplicationUser)
                .WithMany()
                .HasForeignKey(a => a.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HostApplication>()
                .HasOne(a => a.Property)
                .WithMany()
                .HasForeignKey(a => a.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== PropertyImage → Property =====
            builder.Entity<PropertyImage>()
                .HasOne(pi => pi.Property)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== One-to-One: Payment → Booking =====
            builder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== One-to-One: GuestCheckIn → Booking =====
            builder.Entity<GuestCheckIn>()
                .HasOne(g => g.Booking)
                .WithOne(b => b.GuestCheckIn)
                .HasForeignKey<GuestCheckIn>(g => g.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== One-to-One: CheckInProcess → Property =====
            builder.Entity<CheckInProcess>()
                .HasOne(c => c.Property)
                .WithOne(p => p.CheckInProcess)
                .HasForeignKey<CheckInProcess>(c => c.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== One-to-Many: GuestDocument → GuestCheckIn =====
            builder.Entity<GuestDocument>()
                .HasOne(d => d.GuestCheckIn)
                .WithMany(g => g.Documents)
                .HasForeignKey(d => d.GuestCheckInId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Many-to-Many: Property ↔ Amenity =====
            builder.Entity<HostProperty>()
                .HasMany(p => p.Amenities)
                .WithMany(a => a.Properties)
                .UsingEntity(j => j.ToTable("PropertyAmenities"));
        }
    }
}