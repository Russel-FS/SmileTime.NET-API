using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;
using SmileTimeNET_API.src.Domain.Interfaces;
using SmileTimeNET_API.src.Domain.Models;

namespace SmileTimeNET_API.src.Aplication.services.chat
{

    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        /// <summary>
        ///  Inicializas una nueva instancia de la clase ConversationService.
        /// </summary>
        /// <param name="context">El contexto de la base de datos.</param>

        public ConversationService(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Obtiene las conversaciones de un usuario con el  ltimo mensaje de cada una.
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        /// <returns>
        /// Una enumeraci n de <see cref="ConversationWithLastMessage"/> que representa las conversaciones
        /// del usuario con su  ltimo mensaje.
        /// </returns>
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

        /// <summary>
        /// Obtiene una conversación por su ID. Se asegura de que la conversación exista y
        /// que el usuario solicitante sea un participante de la conversación.
        /// </summary>
        /// <param name="conversationId">El ID de la conversación.</param>
        /// <param name="userId">El ID del usuario.</param>
        /// <returns>
        /// A <see cref="Conversation"/> que representa la conversación.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Excepción lanzada cuando la conversación no se encuentra.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Excepción lanzada cuando el usuario solicitante no es un participante de la conversación.
        /// </exception>
        public async Task<ConversationDto> GetConversationByIdAsync(int conversationId, string userId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .ThenInclude(cp => cp.User)
                .Select(c => new ConversationDto
                {
                    ConversationId = c.ConversationId,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Type = c.Type,
                    Title = c.Title,
                    IsActive = c.IsActive,
                    Participants = c.Participants
                        .Where(p => p.User != null)
                        .Select(p => new UserDTO
                        {
                            UserId = p.UserId,
                            ConversationId = p.ConversationId,
                            UserName = p.User!.UserName,
                            Avatar = p.User.Avatar,
                            IsAdmin = p.IsAdmin,
                            JoinedAt = p.JoinedAt
                        }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation == null)
                throw new KeyNotFoundException("Conversación no encontrada");

            if (!await IsUserParticipantAsync(conversationId, userId))
                throw new UnauthorizedAccessException("Usuario no es participante de esta conversación");

            return conversation;
        }

        /// <summary>
        /// Verifica si el usuario con el ID <paramref name="userId"/> es un participante de la conversación con el ID <paramref name="conversationId"/>.
        /// </summary>
        /// <param name="conversationId">El ID de la conversación.</param>
        /// <param name="userId">El ID del usuario.</param>
        /// <returns>
        /// <see langword="true"/> si el usuario es un participante de la conversación, <see langword="false"/> en caso contrario.
        /// </returns>
        public async Task<bool> IsUserParticipantAsync(int conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);
        }


        /// <summary>
        /// Obtiene los participantes de una conversación en formato UserDTO.
        /// </summary>
        /// <param name="conversationId">El ID de la conversación.</param>
        /// <param name="requestingUserId">El ID del usuario que realiza la solicitud.</param>
        /// <returns>
        /// Una colección de <see cref="UserDTO"/> que representa los participantes de la conversación.
        /// </returns>
        /// <exception cref="KeyNotFoundException">Excepción lanzada cuando la conversación no se encuentra.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// Excepción lanzada cuando el usuario solicitante no es un participante de la conversación.
        /// </exception>
        public async Task<IEnumerable<UserDTO>> GetConversationParticipantsAsync(int conversationId, string requestingUserId)
        {
            if (!await IsUserParticipantAsync(conversationId, requestingUserId))
                throw new UnauthorizedAccessException("Usuario no es participante de esta conversación");

            var participants = await _context.ConversationParticipants
                .Where(cp => cp.ConversationId == conversationId)
                .Include(cp => cp.User)
                .Select(cp => new UserDTO
                {
                    UserId = cp.UserId,
                    UserName = cp.User!.UserName,
                    Avatar = cp.User.Avatar,
                    LastActive = cp.User.LastActive,
                    JoinedAt = cp.JoinedAt,
                    LeftAt = cp.LeftAt,
                    Role = cp.IsAdmin ? "admin" : "member",
                    IsOnline = cp.User.IsActive,
                    ConversationId = cp.ConversationId
                })
                .ToListAsync();

            if (!participants.Any())
                throw new KeyNotFoundException("No se encontraron participantes en esta conversación");

            return participants;
        }


        /// <summary>
        /// Obtiene todos los usuarios con los que el usuario ha tenido conversaciones.
        /// </summary>
        /// <param name="userId">El ID del usuario que realiza la consulta.</param>
        /// <returns>
        /// Una colección de <see cref="UserDTO"/> que representa los usuarios con los que se ha conversado.
        /// </returns>
        public async Task<IEnumerable<UserDTO>> GetUserConversationPartnersAsync(string userId)
        {
            var conversationPartners = await _context.ConversationParticipants
                .Where(cp => cp.Conversation.Participants.Any(p => p.UserId == userId))
                .Where(cp => cp.UserId != userId)
                .Include(cp => cp.User)
                .Select(cp => new UserDTO
                {
                    UserId = cp.UserId,
                    UserName = cp.User!.UserName,
                    Avatar = cp.User.Avatar,
                    LastActive = cp.User.LastActive,
                    JoinedAt = cp.JoinedAt,
                    LeftAt = cp.LeftAt,
                    Role = cp.IsAdmin ? "admin" : "member",
                    IsOnline = cp.User.IsActive,
                    ConversationId = cp.ConversationId
                })
                .Distinct()
                .ToListAsync();

            return conversationPartners;
        }



     
        /// <summary>
        /// Crea una nueva conversaci n.
        /// </summary>
        /// <param name="conversation">La conversaci n a crear.</param>
        /// <returns>
        /// La conversaci n creada.
        /// </returns>
        /// <exception cref="ArgumentException">Excepci n lanzada si la conversaci n no tiene al menos un participante.</exception>
        /// <exception cref="KeyNotFoundException">Excepci n lanzada si uno o m s usuarios no existen.</exception>
        /// <exception cref="DbUpdateException">Excepci n lanzada si ocurre un error al guardar la conversaci n.</exception>
        public async Task<ConversationDto> CreateConversationAsync(ConversationDto conversation)
        {
            try
            {
                // Validar que al menos haya un participante
                if (!conversation.Participants.Any())
                {
                    throw new ArgumentException("La conversación debe tener al menos un participante");
                }

                // Verificar que los usuarios existan
                var userIds = conversation.Participants.Select(p => p.UserId).ToList();
                var existingUsers = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToListAsync();

                if (existingUsers.Count != userIds.Count)
                {
                    throw new KeyNotFoundException("Uno o más usuarios no existen");
                }

                // Establecer fechas si no están definidas
                conversation.CreatedAt ??= DateTime.UtcNow;
                conversation.UpdatedAt ??= DateTime.UtcNow;

                var conversationEntity = _mapper.Map<Conversation>(conversation);

                // 
           
                await _context.Conversations.AddAsync(conversationEntity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ConversationDto>(conversationEntity);
            }
            catch (DbUpdateException ex)
            {
                throw new DbUpdateException("Error al guardar la conversación", ex);
            }
        }

        public async Task<IEnumerable<UserDTO>> GetUserDentistsAsync(string userId)
        {
            var users = await _context.Users.ToListAsync();
            var dentists = new List<UserDTO>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Dentist"))
                {
                    dentists.Add(new UserDTO
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Avatar = user.Avatar,
                        LastActive = user.LastActive,
                        Role = "Dentist",
                        IsOnline = user.IsActive
                    });
                }
            }

            return dentists;
        }

        public async Task<IEnumerable<UserDTO>> GetUserPatientsAsync(string userId)
        {
            var users = await _context.Users.ToListAsync();
            var patients = new List<UserDTO>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    patients.Add(new UserDTO
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Avatar = user.Avatar,
                        LastActive = user.LastActive,
                        Role = "User",
                        IsOnline = user.IsActive
                    });
                }
            }

            return patients;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync(string userId)
        {
            // Primero obtenemos los usuarios
            var users = await _context.Users
                .Where(u => u.Id != userId)
                .ToListAsync();

            var userDtos = new List<UserDTO>();

            // Luego procesamos los roles 
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDTO
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Avatar = user.Avatar,
                    LastActive = user.LastActive,
                    Role = roles.FirstOrDefault() ?? "User",
                    IsOnline = user.IsActive
                });
            }

            return userDtos;
        }
    }
}