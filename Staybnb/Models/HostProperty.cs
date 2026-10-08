using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public class HostProperty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price Per Night")]
        public decimal PricePerNight { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string PropertyType { get; set; } = string.Empty;

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int MaxGuests { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key
        [ForeignKey("Host")]
        public string HostId { get; set; } = string.Empty;
        public virtual ApplicationUser? Host { get; set; }

        // Navigation
        public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
        public virtual CheckInProcess? CheckInProcess { get; set; }
    }
}