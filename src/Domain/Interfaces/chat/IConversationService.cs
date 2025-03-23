using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmileTimeNET_API.Models; 
using SmileTimeNET_API.src.Domain.Models;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IConversationService 
    {
        Task<IEnumerable<ConversationWithLastMessage>> GetUserConversationsWithLastMessageAsync(string userId);
        Task<Conversation> GetConversationByIdAsync(int conversationId, string userId);
        Task<bool> IsUserParticipantAsync(int conversationId, string userId);
    }
}