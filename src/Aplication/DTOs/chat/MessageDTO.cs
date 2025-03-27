using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Aplication.DTOs.chat
{
    public class MessageDTO
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public string? Content { get; set; } = string.Empty;
        public string? MessageType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? SenderId { get; set; } = string.Empty;
        public UserDTO? Sender { get; set; } = null!;
        public ICollection<AttachmentDTO>? Attachments { get; set; } = new List<AttachmentDTO>();
        public ICollection<MessageStatusDTO>? MessageStatuses { get; set; } = new List<MessageStatusDTO>();
    }
}
