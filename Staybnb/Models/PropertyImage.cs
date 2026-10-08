using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public class PropertyImage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual HostProperty? Property { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;
    }
}