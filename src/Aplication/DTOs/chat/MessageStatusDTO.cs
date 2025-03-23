using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Aplication.DTOs.chat
{
    public class MessageStatusDTO
    {
        public int MessageId { get; set; }
        public string? Status { get; set; }
        public DateTime StatusTimestamp { get; set; }
    }
}