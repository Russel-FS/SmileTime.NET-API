using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Avatar { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }  
        public DateTime? LastActive { get; set; }
        public bool IsActive { get; set; }

        // Navegaciones
        public ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
    }
}
