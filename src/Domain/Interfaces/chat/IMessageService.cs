using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;
using SmileTimeNET_API.src.Aplication.services;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetMessagesByUserIdAsync(string userId);
        Task<PaginatedResponse<MessageDTO>> GetMessagesByConversationIdAsync(int conversationId, string userId, int page = 1);

        Task<MessageDTO> CreateMessageAsync(Message message);
    }
}