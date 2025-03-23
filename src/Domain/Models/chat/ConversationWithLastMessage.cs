using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.Models;

namespace SmileTimeNET_API.src.Domain.Models
{
    public class ConversationWithLastMessage
    {
        public Conversation? Conversation { get; set; }
        public Message? LastMessage { get; set; }
    }
}