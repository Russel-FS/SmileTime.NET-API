using System;
using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public string? SenderId { get; set; }
        public string? Content { get; set; }
        public string? MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navegaciones
        public Conversation? Conversation { get; set; }
        public ApplicationUser? Sender { get; set; }
        public ICollection<MessageStatus>? MessageStatuses { get; set; }
        public ICollection<Attachment>? Attachments { get; set; }
    }
}
