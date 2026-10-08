using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public class CheckInProcess
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual HostProperty? Property { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public string Rules { get; set; } = string.Empty;

        // JSON array of required documents (e.g., ["ID","Passport"])
        public string RequiredDocuments { get; set; } = "[]";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}