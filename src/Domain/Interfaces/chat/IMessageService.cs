using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetMessagesByUserIdAsync(string userId);
        Task<IEnumerable<MessageDTO>> GetMessagesByConversationIdAsync(int conversationId, string userId);
    }
}