using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActive { get; set; }
        public bool IsActive { get; set; }

        // Navegaciones
        public ICollection<ConversationParticipant>? ConversationParticipants { get; set; }
        public ICollection<Message>? Messages { get; set; }
        public ICollection<MessageStatus>? MessageStatuses { get; set; }
    }
}
