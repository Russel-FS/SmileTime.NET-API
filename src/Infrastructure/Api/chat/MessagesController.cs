using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Aplication.DTOs.chat;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Infrastructure.Api.chat
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Crea un nuevo mensaje en la conversaci n especificada.
        /// </summary>
        /// <param name="message">El mensaje a crear.</param>
        /// <returns>
        /// El mensaje creado.
        /// </returns>
        /// <exception cref="ArgumentException">Excepci n lanzada si el mensaje es nulo o inv lido.</exception>
        /// <exception cref="ApplicationException">Excepci n lanzada si ocurre un error al crear el mensaje.</exception>
        [HttpPost("create")]
        public async Task<IActionResult> CreateMessage([FromBody] MessageDTO message)
        {
            Console.WriteLine("CreateMessage................." + JsonSerializer.Serialize(message));

            if (message == null)
                return BadRequest("Mensaje requerido");

            try
            {
                var createdMessage = await _messageService.CreateMessageAsync(message);
                return Ok(createdMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear el mensaje: {ex.Message}");
            }
        }

    }
}