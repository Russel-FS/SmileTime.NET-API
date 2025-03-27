using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Aplication.DTOs.chat
{
    public class UserDTO
    {
        public string? UserId { get; set; } = string.Empty;
        public string? UserName { get; set; } = string.Empty;
        public string? Avatar { get; set; } = string.Empty;
        public DateTime? LastActive { get; set; }
        public DateTime? JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public string? Role { get; set; }
        public bool? IsOnline { get; set; }
        public int? ConversationId { get; set; }
        public bool? IsAdmin { get; set; }
    }
}

