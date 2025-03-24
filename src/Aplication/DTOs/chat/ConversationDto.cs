using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Aplication.DTOs.chat
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Type { get; set; } 
        public string? Title { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserDTO> Participants { get; set; } = new List<UserDTO>();
        public ICollection<MessageDTO>? Messages { get; set; } = new List<MessageDTO>();
    }
}