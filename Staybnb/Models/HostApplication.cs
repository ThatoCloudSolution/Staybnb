using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class HostApplication
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual HostProperty? Property { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public string? AdminNotes { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.Now;
    }
}