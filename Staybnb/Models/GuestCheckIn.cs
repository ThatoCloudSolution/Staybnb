using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum CheckInStatus
    {
        Pending,
        DocumentsSubmitted,
        Verified,
        Rejected
    }

    public class GuestCheckIn
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public virtual Booking? Booking { get; set; }

        [ForeignKey("CheckInProcess")]
        public int CheckInProcessId { get; set; }
        public virtual CheckInProcess? CheckInProcess { get; set; }

        public CheckInStatus Status { get; set; } = CheckInStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<GuestDocument> Documents { get; set; } = new List<GuestDocument>();
    }
}