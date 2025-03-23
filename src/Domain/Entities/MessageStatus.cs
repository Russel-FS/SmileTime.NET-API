using System;

namespace SmileTimeNET_API.Models
{
    public class MessageStatus
    {
        public int MessageId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StatusTimestamp { get; set; }  

        // Navegaciones
        public Message Message { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
