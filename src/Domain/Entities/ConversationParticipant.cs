using System;

namespace SmileTimeNET_API.Models
{
    public class ConversationParticipant
    {
        public int ConversationId { get; set; }
        public string? UserId { get; set; }
        public DateTime JoinedAt { get; set; }   
        public DateTime? LeftAt { get; set; }
        public DateTime? LastRead { get; set; } 
        public bool IsAdmin { get; set; }

        // Navegaciones
        public Conversation? Conversation { get; set; } = null!;
        public ApplicationUser? User { get; set; } = null!;
    }
}
