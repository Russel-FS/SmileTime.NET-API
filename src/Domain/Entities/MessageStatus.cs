using System;

namespace SmileTimeNET_API.Models
{
    public class MessageStatus
    {
        public int MessageId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StatusTimestamp { get; set; }

        // Navegaciones opcionales
        public virtual Message? Message { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
