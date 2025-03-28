using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Domain.Models.signalR
{
    public class OnlineUserDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
    }
}