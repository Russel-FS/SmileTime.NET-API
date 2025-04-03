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
        public async Task<IActionResult> CreateAppointment([FromBody] DentalAppointmentDto appointmentDto)
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
                    Status = "Pending" // Estado inicial
                };

                _logger.LogInformation($"Appointment received for patient {appointmentDto.PatientId} on {appointmentDto.Date} at {appointmentDto.Time}");


                return Ok(new { message = "Appointment created successfully", appointment });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
