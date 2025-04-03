using Microsoft.EntityFrameworkCore;
using SmileTimeNET.Domain.Entities.Dentist;
using SmileTimeNET.Infrastructure.Api.Dentist;
using SmileTimeNET_API.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SmileTimeNET.Application.Services.Dentist
{
    public class DentalAppointmentService : IDentalAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DentalAppointmentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string DENTIST_ROLE = "Dentist";

        public DentalAppointmentService(
            ApplicationDbContext context,
            ILogger<DentalAppointmentService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentDentistId()
        {
            var dentistId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(dentistId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el ID del dentista del token");
            }
            return dentistId;
        }

        private bool IsDentist()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(DENTIST_ROLE) ?? false;
        }

        private void ValidateDentistRole()
        {
            if (!IsDentist())
            {
                throw new UnauthorizedAccessException("Solo los dentistas pueden realizar esta operación");
            }
        }

        public async Task<DentalAppointment> CreateAppointmentAsync(DentalAppointmentDto appointmentDto)
        {
            ValidateDentistRole();
            var dentistId = GetCurrentDentistId();

            var appointment = new DentalAppointment
            {
                Date = appointmentDto.Date,
                Time = appointmentDto.Time,
                Duration = appointmentDto.Duration,
                Notes = appointmentDto.Notes ?? string.Empty,
                PatientId = appointmentDto.PatientId,
                DentistId = dentistId, 
                Type = appointmentDto.Type,
                Status = "Pending"
            };

            await _context.DentalAppointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<DentalAppointment> UpdateAppointmentAsync(int id, DentalAppointmentDto appointmentDto)
        {
            ValidateDentistRole();
            var appointment = await _context.DentalAppointments
                .FirstOrDefaultAsync(a => a.Id == id && a.DentistId == GetCurrentDentistId());

            if (appointment == null)
                throw new KeyNotFoundException($"Cita con ID {id} no encontrada o no pertenece al dentista actual");

            appointment.Date = appointmentDto.Date;
            appointment.Time = appointmentDto.Time;
            appointment.Duration = appointmentDto.Duration;
            appointment.Notes = appointmentDto.Notes ?? string.Empty;
            appointment.Type = appointmentDto.Type;

            _context.DentalAppointments.Update(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            ValidateDentistRole();
            var appointment = await _context.DentalAppointments
                .FirstOrDefaultAsync(a => a.Id == id && a.DentistId == GetCurrentDentistId());

            if (appointment == null)
                return false;

            _context.DentalAppointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        private DentalAppointmentResponseDto MapToResponseDto(DentalAppointment appointment)
        {
            return new DentalAppointmentResponseDto
            {
                Appointment = new AppointmentDto
                {
                    PatientId = appointment.PatientId,
                    Date = appointment.Date.ToString("yyyy-MM-dd"),
                    Time = appointment.Time,
                    Type = appointment.Type,
                    Duration = appointment.Duration,
                    Notes = appointment.Notes
                },
                PatientInfo = new PatientInfoDto
                {
                    Name = appointment.Patient?.UserName ?? string.Empty,
                    Phone = appointment.Patient?.PhoneNumber ?? string.Empty,
                    Status = appointment.Patient?.IsActive == true ? "active" : "inactive"
                }
            };
        }

        public async Task<DentalAppointmentResponseDto> GetAppointmentByIdAsync(int id)
        {
            ValidateDentistRole();
            var appointment = await _context.DentalAppointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id && a.DentistId == GetCurrentDentistId());

            if (appointment == null)
                throw new KeyNotFoundException($"Cita con ID {id} no encontrada o no pertenece al dentista actual");

            return MapToResponseDto(appointment);
        }

        public async Task<IEnumerable<DentalAppointmentResponseDto>> GetAppointmentsByDentistIdAsync(string dentistId)
        {
            ValidateDentistRole();
            var currentDentistId = GetCurrentDentistId();

            if (dentistId != currentDentistId)
            {
                throw new UnauthorizedAccessException("Solo puede ver sus propias citas");
            }

            var appointments = await _context.DentalAppointments
                .Include(a => a.Patient)
                .Where(a => a.DentistId == dentistId)
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Time)
                .ToListAsync();

            return appointments.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<DentalAppointmentResponseDto>> GetAppointmentsByPatientIdAsync(string patientId)
        {
            ValidateDentistRole();
            var appointments = await _context.DentalAppointments
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId && a.DentistId == GetCurrentDentistId())
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Time)
                .ToListAsync();

            return appointments.Select(MapToResponseDto);
        }
    }
}
