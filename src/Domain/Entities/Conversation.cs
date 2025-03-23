using System;
using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public DateTime CreatedAt { get; set; }   
        public DateTime? UpdatedAt { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Navegaciones
        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
