using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;
using SmileTimeNET_API.src.Domain.Models;

namespace SmileTimeNET_API.src.Aplication.services.chat
{

    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext _context;

        public ConversationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ConversationWithLastMessage>> GetUserConversationsWithLastMessageAsync(string userId)
        {
            return await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => new ConversationWithLastMessage
                {
                    Conversation = cp.Conversation,
                    LastMessage = cp.Conversation != null && cp.Conversation.Messages != null ? cp.Conversation.Messages
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.CreatedAt)
                        .FirstOrDefault() : null

                })
                .ToListAsync();
        }
        public async Task<Conversation> GetConversationByIdAsync(int conversationId, string userId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .ThenInclude(cp => cp.User)
                .Select(c => new Conversation
                {
                    ConversationId = c.ConversationId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Type = c.Type,
                    Title = c.Title,
                    IsActive = c.IsActive,
                    Participants = c.Participants.Select(p => new ConversationParticipant
                    {
                        UserId = p.UserId,
                        ConversationId = p.ConversationId,
                        User = new ApplicationUser
                        {
                            UserName = p.User.UserName,
                            Avatar = p.User.Avatar
                        },
                        IsAdmin = p.IsAdmin,
                        JoinedAt = p.JoinedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                throw new KeyNotFoundException("Conversación no encontrada");

            if (!await IsUserParticipantAsync(conversationId, userId))
                throw new UnauthorizedAccessException("Usuario no es participante de esta conversación");

            return conversation;
        }

        public async Task<bool> IsUserParticipantAsync(int conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);
        }
    }
}