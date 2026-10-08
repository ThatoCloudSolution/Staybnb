using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public class WishlistItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual HostProperty? Property { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}