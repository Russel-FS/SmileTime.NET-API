using System;
using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }  
        public DateTime? ModifiedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navegaciones
        public Conversation Conversation { get; set; } = null!;
        public ApplicationUser Sender { get; set; } = null!;
        public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
