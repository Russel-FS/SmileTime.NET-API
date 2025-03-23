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
        private const int DefaultPageSize = 50;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponse<MessageDTO>> GetMessagesByConversationIdAsync(int conversationId, string userId, int page = 1)
        {
            if (page < 1) page = 1;

            if (conversationId <= 0)
                throw new ArgumentException("ID de conversación inválido");
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("ID de usuario requerido");

            try
            {
                var isParticipant = await _context.ConversationParticipants
                    .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

                if (!isParticipant)
                {
                    throw new UnauthorizedAccessException("Usuario no es participante de esta conversación");
                }

                var totalMessages = await _context.Messages
                    .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                    .CountAsync();

                var skip = (page - 1) * DefaultPageSize;

                var messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.MessageStatuses)
                    .Include(m => m.Attachments)
                    .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedAt)
                    .Skip(skip)
                    .Take(DefaultPageSize)
                    .ToListAsync();

                return new PaginatedResponse<MessageDTO>
                {
                    Items = messages.Select(MapToMessageDTO),
                    CurrentPage = page,
                    PageSize = DefaultPageSize,
                    TotalItems = totalMessages,
                    TotalPages = (int)Math.Ceiling(totalMessages / (double)DefaultPageSize)
                };
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                throw new ApplicationException("Error al obtener mensajes", ex);
            }
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
                        UserId = m.Sender != null ? m.Sender.Id : string.Empty,
                        UserName = m.Sender.UserName ?? string.Empty,
                        Avatar = m.Sender.Avatar ?? string.Empty
                    },
                    Attachments = m.Attachments.Select(a => new AttachmentDTO
                    {
                        MessageId = a.MessageId,
                        AttachmentId = a.AttachmentId,
                        FileUrl = a.FileUrl ?? string.Empty,
                        FileType = a.FileType ?? string.Empty
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

        private static MessageDTO MapToMessageDTO(Message message)
        {
            if (message == null) return new MessageDTO
            {
                Attachments = new List<AttachmentDTO>(),
                MessageStatuses = new List<MessageStatusDTO>()
            };

            return new MessageDTO
            {
                MessageId = message.MessageId,
                ConversationId = message.ConversationId,
                Content = message.Content ?? string.Empty,
                MessageType = message.MessageType,
                CreatedAt = message.CreatedAt,
                ModifiedAt = message.ModifiedAt,
                SenderId = message.SenderId,
                Sender = new UserDTO
                {
                    UserId = message.Sender?.Id ?? string.Empty,
                    UserName = message.Sender?.UserName ?? string.Empty,
                    Avatar = message.Sender?.Avatar ?? string.Empty
                },
                Attachments = message.Attachments?.Select(a => new AttachmentDTO
                {
                    MessageId = a.MessageId,
                    AttachmentId = a.AttachmentId,
                    FileUrl = a.FileUrl ?? string.Empty,
                    FileType = a.FileType ?? string.Empty
                }).ToList() ?? new List<AttachmentDTO>(),
                MessageStatuses = message.MessageStatuses?.Select(ms => new MessageStatusDTO
                {
                    MessageId = ms.MessageId,
                    Status = ms.Status,
                    StatusTimestamp = ms.StatusTimestamp
                }).ToList() ?? new List<MessageStatusDTO>()
            };
        }

    }

    public class PaginatedResponse<T>
    {
        public IEnumerable<T>? Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
    }
}

