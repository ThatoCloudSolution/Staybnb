using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum BookingStatus
    {
        Pending,
        Approved,
        Rejected,
        CheckedIn,
        Cancelled
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual HostProperty? Property { get; set; }

        [ForeignKey("Guest")]
        public string GuestId { get; set; } = string.Empty;
        public virtual ApplicationUser? Guest { get; set; }

        [Required]
        [Display(Name = "Check In")]
        public DateTime CheckInDate { get; set; }

        [Required]
        [Display(Name = "Check Out")]
        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CleaningFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ServiceFee { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public virtual Payment? Payment { get; set; }
        public virtual GuestCheckIn? GuestCheckIn { get; set; }
    }
}