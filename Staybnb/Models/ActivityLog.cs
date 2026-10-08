using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum ActivityType
    {
        Login,
        Logout,
        Registration,
        RoleChange,
        BookingStatusChange,
        ApplicationSubmitted,
        ApplicationApproved,
        ApplicationRejected,
        PropertyCreated,
        PropertyUpdated
    }

    public class ActivityLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        public ActivityType ActivityType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}