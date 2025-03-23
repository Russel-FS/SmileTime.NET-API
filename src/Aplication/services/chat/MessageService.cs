using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;
using SmileTimeNET_API.src.Domain.Interfaces;
using SmileTimeNET_API.src.Domain.Models;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<MessageDTO>> GetMessagesByConversationIdAsync(int conversationId, string userId)
        {
            // Primero verificamos si el usuario es participante de la conversación
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                throw new UnauthorizedAccessException("Usuario no es participante de esta conversación");
            }

            return await _context.Messages
           .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
           .Select(m => new MessageDTO
           {
               MessageId = m.MessageId,
               ConversationId = m.ConversationId,
               Content = m.Content,
               MessageType = m.MessageType,
               CreatedAt = m.CreatedAt,
               ModifiedAt = m.ModifiedAt,
               SenderId = m.SenderId,
               Sender = new UserDTO
               {
                   UserId = m.Sender.Id,
                   UserName = m.Sender.UserName,
                   Avatar = m.Sender.Avatar
               },
               Attachments = m.Attachments.Select(a => new AttachmentDTO
               {
                   MessageId = a.MessageId,
                   AttachmentId = a.AttachmentId,
                   FileUrl = a.FileUrl,
                   FileType = a.FileType
               }).ToList(),
               MessageStatuses = m.MessageStatuses.Select(ms => new MessageStatusDTO
               {
                   MessageId = ms.MessageId,
                   Status = ms.Status,
                   StatusTimestamp = ms.StatusTimestamp
               }).ToList()
           })
         .OrderBy(m => m.CreatedAt)
         .ToListAsync();
        }

        public async Task<IEnumerable<MessageDTO>> GetMessagesByUserIdAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.MessageStatuses)
                .Include(m => m.Attachments)
                .Where(m => m.SenderId == userId && !m.IsDeleted)
                .Select(m => new MessageDTO
                {
                    MessageId = m.MessageId,
                    ConversationId = m.ConversationId,
                    Content = m.Content,
                    MessageType = m.MessageType,
                    CreatedAt = m.CreatedAt,
                    ModifiedAt = m.ModifiedAt,
                    SenderId = m.SenderId,
                    Sender = new UserDTO
                    {
                        UserId = m.Sender.Id,
                        UserName = m.Sender.UserName,
                        Avatar = m.Sender.Avatar
                    },
                    Attachments = m.Attachments.Select(a => new AttachmentDTO
                    {
                        MessageId = a.MessageId,
                        AttachmentId = a.AttachmentId,
                        FileUrl = a.FileUrl,
                        FileType = a.FileType
                    }).ToList(),
                    MessageStatuses = m.MessageStatuses.Select(ms => new MessageStatusDTO
                    {
                        MessageId = ms.MessageId,
                        Status = ms.Status,
                        StatusTimestamp = ms.StatusTimestamp
                    }).ToList()
                })
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }

}

