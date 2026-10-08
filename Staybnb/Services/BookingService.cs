using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;

namespace Staybnb.Services
{
    public class BookingService
    {
        private const decimal SERVICE_FEE_RATE = 0.10m;  // 10% service fee
        private const decimal CLEANING_FEE = 350m;      // Fixed cleaning fee in ZAR

        private readonly ApplicationDbContext _context;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calculates TotalPrice = (PricePerNight × Nights) + CleaningFee + ServiceFee
        /// </summary>
        public decimal CalculateTotalPrice(decimal pricePerNight, int nights)
        {
            var subtotal = pricePerNight * nights;
            var cleaningFee = CLEANING_FEE;
            var serviceFee = subtotal * SERVICE_FEE_RATE;
            return subtotal + cleaningFee + serviceFee;
        }

        public decimal GetCleaningFee() => CLEANING_FEE;

        public async Task<Booking> CreateBookingAsync(string guestId, int propertyId,
            DateTime checkIn, DateTime checkOut, int numberOfGuests)
        {
            var property = await _context.HostProperties.FindAsync(propertyId)
                ?? throw new InvalidOperationException("Property not found.");

            var nights = (checkOut - checkIn).Days;
            if (nights <= 0) throw new InvalidOperationException("Invalid dates.");

            var cleaningFee = CLEANING_FEE;
            var serviceFee = property.PricePerNight * nights * SERVICE_FEE_RATE;

            var booking = new Booking
            {
                PropertyId = propertyId,
                GuestId = guestId,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NumberOfGuests = numberOfGuests,
                CleaningFee = cleaningFee,
                ServiceFee = serviceFee,
                TotalPrice = CalculateTotalPrice(property.PricePerNight, nights),
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);

            // Notify host
            _context.Notifications.Add(new Notification
            {
                UserId = property.HostId,
                Title = "New Booking Request",
                Message = $"You have a new booking request for '{property.Title}'.",
                Type = NotificationType.NewBookingRequest
            });

            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> ApproveBookingAsync(int bookingId, string hostId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Property?.HostId != hostId)
                return false;

            booking.Status = BookingStatus.Approved;

            _context.Notifications.Add(new Notification
            {
                UserId = booking.GuestId,
                Title = "Booking Approved",
                Message = $"Your booking for '{booking.Property.Title}' has been approved!",
                Type = NotificationType.BookingApproved
            });

            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = hostId,
                Action = $"Approved booking ID {bookingId}",
                ActivityType = ActivityType.BookingStatusChange
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectBookingAsync(int bookingId, string hostId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null || booking.Property?.HostId != hostId)
                return false;

            booking.Status = BookingStatus.Rejected;

            _context.Notifications.Add(new Notification
            {
                UserId = booking.GuestId,
                Title = "Booking Rejected",
                Message = $"Unfortunately, your booking for '{booking.Property.Title}' was rejected.",
                Type = NotificationType.BookingRejected
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}