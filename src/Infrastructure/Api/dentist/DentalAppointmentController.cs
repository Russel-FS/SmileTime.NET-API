using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET.Application.Services.Dentist;
using SmileTimeNET.Domain.Entities.Dentist;

namespace SmileTimeNET.Infrastructure.Api.Dentist
{
    [Authorize(Roles = "Dentist")]
    [ApiController]
    [Route("api/dental-appointments")]
    public class DentalAppointmentController : ControllerBase
    {
        private readonly ILogger<DentalAppointmentController> _logger;
        private readonly IDentalAppointmentService _appointmentService;

        public DentalAppointmentController(
            ILogger<DentalAppointmentController> logger,
            IDentalAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<DentalAppointment>> CreateAppointment([FromBody] DentalAppointmentDto appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(appointmentDto);
                return Ok(appointment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la cita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<DentalAppointment>> UpdateAppointment(int id, [FromBody] DentalAppointmentDto appointmentDto)
        {
            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(id, appointmentDto);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteAppointment(int id)
        {
            try
            {
                var result = await _appointmentService.DeleteAppointmentAsync(id);
                if (!result)
                    return NotFound(new { message = $"Cita con ID {id} no encontrada" });

                return Ok(new { message = "Cita eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la cita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult<DentalAppointment>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return Ok(appointment);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cita");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("my-appointments")]
        public async Task<ActionResult<IEnumerable<DentalAppointment>>> GetMyAppointments()
        {
            try
            {
                var dentistId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var appointments = await _appointmentService.GetAppointmentsByDentistIdAsync(dentistId ?? string.Empty);
                return Ok(appointments);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las citas del dentista");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("by-patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<DentalAppointment>>> GetPatientAppointments(string patientId)
        {
            try
            {

                var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
                return Ok(appointments);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las citas del paciente");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }
}
