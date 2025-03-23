using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class MessageService 
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(int conversationId, string userId)
        {
            // Primero verificamos si el usuario es participante de la conversación
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                throw new UnauthorizedAccessException("Usuario no es participante de esta conversación");
            }

            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.MessageStatuses)
                .Include(m => m.Attachments)
                .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConversationWithLastMessage>> GetUserConversationsWithLastMessageAsync(string userId)
        {
            var conversations = await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => new ConversationWithLastMessage
                {
                    Conversation = cp.Conversation,
                    LastMessage = cp.Conversation.Messages 
                        .Where(m => !m.IsDeleted)
                        .OrderByDescending(m => m.CreatedAt)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return conversations;
        }
    }

    public class ConversationWithLastMessage
    {
        public Conversation? Conversation { get; set; }
        public Message? LastMessage { get; set; }
    }
}