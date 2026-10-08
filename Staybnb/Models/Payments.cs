using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Booking")]
        public int BookingId { get; set; }
        public virtual Booking? Booking { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}