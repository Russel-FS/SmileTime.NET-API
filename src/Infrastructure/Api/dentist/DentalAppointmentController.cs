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

        [HttpPost]
        public async Task<ActionResult<AppointmentResponse>> CreateAppointment([FromBody] DentalAppointmentDto appointmentDto)
        {
            try
            {
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

                _logger.LogInformation($"Appointment received for patient {appointmentDto.PatientId}");

                var response = new AppointmentResponse
                {
                    Success = true,
                    Message = "Cita creada exitosamente",
                    Data = new AppointmentData
                    {
                        Date = appointment.Date,
                        Time = appointment.Time,
                        Duration = appointment.Duration,
                        Type = appointment.Type,
                        Status = appointment.Status,
                        PatientId = appointment.PatientId,
                        Notes = appointment.Notes,
                        PatientInfo = appointmentDto.PatientInfo != null ? new PatientInfo
                        {
                            Name = appointmentDto.PatientInfo.Name,
                            Phone = appointmentDto.PatientInfo.Phone,
                            Status = appointmentDto.PatientInfo.Status
                        } : null
                    }
                };

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
