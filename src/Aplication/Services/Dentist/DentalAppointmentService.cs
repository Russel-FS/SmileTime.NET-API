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

        public async Task<DentalAppointment> CreateAppointmentAsync(DentalAppointmentDto appointmentDto)
        {
            var dentistId = GetCurrentDentistId();
            
            var appointment = new DentalAppointment
            {
                Date = appointmentDto.Date,
                Time = appointmentDto.Time,
                Duration = appointmentDto.Duration,
                Notes = appointmentDto.Notes ?? string.Empty,
                PatientId = appointmentDto.PatientId,
                DentistId = dentistId, // Usamos el ID del dentista del token
                Type = appointmentDto.Type,
                Status = "Pending"
            };

            await _context.DentalAppointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            
            return appointment;
        }

        public async Task<DentalAppointment> UpdateAppointmentAsync(int id, DentalAppointmentDto appointmentDto)
        {
            var appointment = await _context.DentalAppointments.FindAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Cita con ID {id} no encontrada");

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
            var appointment = await _context.DentalAppointments.FindAsync(id);
            if (appointment == null)
                return false;

            _context.DentalAppointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DentalAppointment> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _context.DentalAppointments
                .Include(a => a.Patient)
                .Include(a => a.Dentist)
                .FirstOrDefaultAsync(a => a.Id == id);

            return appointment ?? throw new KeyNotFoundException($"Cita con ID {id} no encontrada");
        }

        public async Task<IEnumerable<DentalAppointment>> GetAppointmentsByDentistIdAsync(string dentistId)
        {
            return await _context.DentalAppointments
                .Include(a => a.Patient)
                .Where(a => a.DentistId == dentistId)
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Time)
                .ToListAsync();
        }

        public async Task<IEnumerable<DentalAppointment>> GetAppointmentsByPatientIdAsync(string patientId)
        {
            return await _context.DentalAppointments
                .Include(a => a.Dentist)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Time)
                .ToListAsync();
        }
    }
}
