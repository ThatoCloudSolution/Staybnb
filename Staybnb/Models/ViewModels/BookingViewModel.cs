using System.ComponentModel.DataAnnotations;

namespace Staybnb.Models.ViewModels
{
    public class BookingViewModel
    {
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check In Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);

        [Required, DataType(DataType.Date)]
        [Display(Name = "Check Out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(3);

        [Required, Range(1, 20)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 1;

        public decimal EstimatedTotal { get; set; }
    }
}