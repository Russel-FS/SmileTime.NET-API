using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.Models
{
    public class ChatMessage
    {
        public string? SenderId { get; set; }
        public string? SenderUserName { get; set; }
        public string? RecipientId { get; set; }
        public string? Content { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}