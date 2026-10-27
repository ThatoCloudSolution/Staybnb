using System.ComponentModel.DataAnnotations;

namespace Staybnb.Models.ViewModels
{
    public class HostApplicationViewModel
    {
        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required, Range(1, 1000000)]
        [Display(Name = "Price Per Night (ZAR)")]
        public decimal PricePerNight { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Property Type")]
        public string PropertyType { get; set; } = string.Empty;

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        [Display(Name = "Max Guests")]
        public int MaxGuests { get; set; }

        [Required(ErrorMessage = "At least one property image is required.")]
        [Display(Name = "Property Image")]
        public IFormFile? Image { get; set; }
    }
}