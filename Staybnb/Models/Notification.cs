using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum NotificationType
    {
        BookingApproved,
        BookingRejected,
        NewMessage,
        ApplicationApproved,
        ApplicationRejected,
        NewBookingRequest
    }

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}