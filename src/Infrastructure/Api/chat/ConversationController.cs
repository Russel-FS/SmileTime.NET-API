using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Infrastructure.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;

        public ConversationController(IConversationService conversationService, IMessageService messageService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
        }

        /// <summary>
        /// Obtiene las conversaciones del usuario autenticado con el ltimo mensaje de cada una.
        /// </summary>
        /// <returns>
        /// Una enumeraci n de <see cref="ConversationWithLastMessage"/> que representa las conversaciones
        /// del usuario autenticado con su ltimo mensaje.
        /// </returns>
        /// <response code="200">Conversaciones encontradas.</response>
        /// <response code="401">Usuario no autenticado.</response>
        /// <response code="500">Error al obtener las conversaciones.</response>
        [HttpGet("Conversations")]
        public async Task<IActionResult> GetUserConversations()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var conversations = await _conversationService.GetUserConversationsWithLastMessageAsync(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las conversaciones", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una conversaci n por su ID. Se asegura de que la conversaci n exista y
        /// que el usuario solicitante sea un participante de la conversaci n.
        /// </summary>
        /// <param name="conversationId">El ID de la conversaci n.</param>
        /// <returns>
        /// Un objeto <see cref="Conversation"/> que representa la conversaci n y una lista de
        /// <see cref="MessageDTO"/> que representa los mensajes de la conversaci n.
        /// </returns>
        /// <response code="200">Conversaci n encontrada.</response>
        /// <response code="401">Usuario no autenticado o no es participante de la conversaci n.</response>
        /// <response code="404">Conversaci n no encontrada.</response>
        /// <response code="500">Error al obtener la conversaci n.</response>
        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationById(int conversationId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var conversation = await _conversationService.GetConversationByIdAsync(conversationId, userId);
                var messages = await _messageService.GetMessagesByConversationIdAsync(conversationId, userId);

                return Ok(new
                {
                    conversation,
                    messages
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la conversación", error = ex.Message });
            }
        }


        /// <summary>
        /// Obtiene todos los usuarios con los que el usuario ha tenido conversaciones.
        /// </summary>
        /// <returns>
        /// Una enumeraci n de <see cref="ConversationWithLastMessage"/> que representa las conversaciones
        /// del usuario autenticado con su ltimo mensaje.
        /// </returns>
        /// <response code="200">Usuarios encontrados.</response>
        /// <response code="401">Usuario no autenticado.</response>
        /// <response code="500">Error al obtener los usuarios.</response>
        [HttpGet("Users")]
        public async Task<IActionResult> GetUserConversationPartners()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var users = await _conversationService.GetUserConversationPartnersAsync(userId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los usuarios", error = ex.Message });
            }
        }
    }
}