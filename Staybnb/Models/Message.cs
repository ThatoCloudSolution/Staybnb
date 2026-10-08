using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Staybnb.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sender")]
        public string SenderId { get; set; } = string.Empty;
        public virtual ApplicationUser? Sender { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; } = string.Empty;
        public virtual ApplicationUser? Receiver { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}