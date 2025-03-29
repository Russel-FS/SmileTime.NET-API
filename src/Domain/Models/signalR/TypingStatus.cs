using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Domain.Models.signalR
{
    public class TypingStatus
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public bool IsTyping { get; set; }
        public int? ConversationId { get; set; }
        public string? Username { get; set; } = string.Empty;
    }
}