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
using AutoMapper;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private const int DefaultPageSize = 50;

        /// <summary>
        /// Constructor del servicio de mensajes.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>
        /// <param name="mapper">El mapeador de objetos.</param>
        public MessageService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene los mensajes de una conversación por su ID, paginados.
        /// </summary>
        /// <param name="conversationId">El ID de la conversación.</param>
        /// <param name="userId">El ID del usuario.</param>
        /// <param name="page">El n mero de p gina a obtener. Si no se especifica, se devuelve la primera p gina.</param>
        /// <returns>
        /// Un objeto <see cref="PaginatedResponse{MessageDTO}"/> que contiene los mensajes de la conversaci n,
        /// la p gina actual, el tama o de p gina (por defecto 50), el total de elementos y el total de p ginas.
        /// </returns>
        /// <exception cref="ArgumentException">Excepci n lanzada si el ID de conversaci n es inv lido.</exception>
        /// <exception cref="UnauthorizedAccessException">Excepci n lanzada si el usuario no es participante de la conversaci n.</exception>
        /// <exception cref="ApplicationException">Excepci n lanzada si ocurre un error al obtener los mensajes.</exception>
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
                    .ThenBy(m => m.MessageId)
                    .Skip(skip)
                    .Take(DefaultPageSize)
                    .Select(m => new MessageDTO
                    {
                        MessageId = m.MessageId,
                        ConversationId = m.ConversationId,
                        Content = m.Content ?? string.Empty,
                        MessageType = m.MessageType ?? string.Empty,
                        CreatedAt = m.CreatedAt,
                        ModifiedAt = m.ModifiedAt,
                        SenderId = m.SenderId ?? string.Empty,
                        Sender = m.Sender == null ? new UserDTO() : new UserDTO
                        {
                            UserId = m.Sender.Id ?? string.Empty,
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
                            Status = ms.Status ?? string.Empty,
                            StatusTimestamp = ms.StatusTimestamp
                        }).ToList()
                    })
                    .ToListAsync();

                return new PaginatedResponse<MessageDTO>
                {
                    Items = messages,
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

        /// <summary>
        /// Obtiene los mensajes enviados por un usuario por su ID.
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        /// <returns>
        /// Una enumeraci n de <see cref="MessageDTO"/> que representa los mensajes enviados por el usuario.
        /// </returns>
        /// <exception cref="ArgumentException">Excepci n lanzada si el ID de usuario es inv lido.</exception>
        /// <exception cref="ApplicationException">Excepci n lanzada si ocurre un error al obtener los mensajes.</exception>
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
                        UserName = m.Sender != null ? m.Sender.UserName ?? string.Empty : string.Empty,
                        Avatar = m.Sender != null ? m.Sender.Avatar ?? string.Empty : string.Empty
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

        /// <summary>
        /// Crea un nuevo mensaje.
        /// </summary>
        /// <param name="message">El mensaje a crear.</param>
        /// <returns>
        /// El mensaje creado.
        /// </returns>
        /// <exception cref="ArgumentException">Excepci n lanzada si el mensaje es nulo o inv lido.</exception>
        /// <exception cref="ApplicationException">Excepci n lanzada si ocurre un error al crear el mensaje.</exception>
        public async Task<MessageDTO> CreateMessageAsync(MessageDTO dto)
        {
            Message message = _mapper.Map<Message>(dto);

            if (message == null)
                throw new ArgumentException("Mensaje requerido");

            if (message.MessageId != 0)
                throw new ArgumentException("Para crear un nuevo mensaje, el MessageId debe ser 0");

            if (message.ConversationId <= 0)
                throw new ArgumentException("ID de conversación inválido");

            try
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                //Console.WriteLine("Mensaje creado con éxito" + message);
                return _mapper.Map<MessageDTO>(message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al crear mensaje", ex);
            }
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

