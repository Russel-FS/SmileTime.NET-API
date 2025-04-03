using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET.Domain.Entities.Dentist;

namespace SmileTimeNET.Infrastructure.Api.Dentist
{
    [ApiController]
    [Route("api/[controller]")]
    public class DentalAppointmentController : ControllerBase
    {
        private readonly ILogger<DentalAppointmentController> _logger;

        public DentalAppointmentController(ILogger<DentalAppointmentController> logger)
        {
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<AppointmentResponse>> CreateAppointment([FromBody] DentalAppointmentDto appointmentDto)
        {
            try
            {
                // Primero vamos a loguear los datos recibidos
                _logger.LogInformation($"Datos recibidos: {JsonSerializer.Serialize(appointmentDto)}");

                // Validamos que los datos requeridos no sean nulos
                if (appointmentDto == null)
                {
                    return BadRequest(new AppointmentResponse 
                    { 
                        Success = false, 
                        Message = "Los datos de la cita son requeridos" 
                    });
                }

                var appointment = new DentalAppointment
                {
                    Date = appointmentDto.Date,
                    Time = appointmentDto.Time,
                    Duration = appointmentDto.Duration,
                    Notes = appointmentDto.Notes ?? string.Empty,
                    PatientId = appointmentDto.PatientId,
                    Type = appointmentDto.Type,
                    Status = "Pending"
                };

                // Creamos la respuesta asegurándonos de usar los datos del DTO
                var response = new AppointmentResponse
                {
                    Success = true,
                    Message = "Cita creada exitosamente",
                    Data = new AppointmentData
                    {
                        Date = appointmentDto.Date,
                        Time = appointmentDto.Time,
                        Duration = appointmentDto.Duration,
                        Type = appointmentDto.Type,
                        Status = "Pending",
                        PatientId = appointmentDto.PatientId,
                        Notes = appointmentDto.Notes ?? string.Empty,
                        PatientInfo = new PatientInfo
                        {
                            Name = appointmentDto.PatientInfo?.Name ?? string.Empty,
                            Phone = appointmentDto.PatientInfo?.Phone ?? string.Empty,
                            Status = appointmentDto.PatientInfo?.Status ?? "active"
                        }
                    }
                };

                // Logueamos la respuesta para verificar
                _logger.LogInformation($"Respuesta: {JsonSerializer.Serialize(response)}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la cita");
                return StatusCode(500, new AppointmentResponse
                {
                    Success = false,
                    Message = "Error interno del servidor"
                });
            }
        }
    }
}
