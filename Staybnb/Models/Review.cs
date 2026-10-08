using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual HostProperty? Property { get; set; }

        [ForeignKey("Reviewer")]
        public string ReviewerId { get; set; } = string.Empty;
        public virtual ApplicationUser? Reviewer { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}