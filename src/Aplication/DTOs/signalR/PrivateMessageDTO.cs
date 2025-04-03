using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.src.Aplication.DTOs.chat;

namespace SmileTimeNET_API.src.Aplication.DTOs.signalR
{
    public class PrivateMessageDTO : MessageDTO
    {
        public string RecipientId { get; set; } = string.Empty;
    }
}