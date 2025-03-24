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
    }
}