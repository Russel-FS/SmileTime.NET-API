using System;

namespace SmileTimeNET_API.Models
{
    public class MessageStatus
    {
        public int MessageId { get; set; }
        public string? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime StatusTimestamp { get; set; }

        // Navegaciones
        public Message? Message { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
