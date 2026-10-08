using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum DocumentStatus
    {
        Pending,
        Verified,
        Rejected
    }

    public class GuestDocument
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("GuestCheckIn")]
        public int GuestCheckInId { get; set; }
        public virtual GuestCheckIn? GuestCheckIn { get; set; }

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    }
}