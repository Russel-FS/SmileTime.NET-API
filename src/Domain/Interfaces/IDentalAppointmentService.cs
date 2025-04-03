using SmileTimeNET.Domain.Entities.Dentist;
using SmileTimeNET.Infrastructure.Api.Dentist;

namespace SmileTimeNET.Application.Services.Dentist
{
    public interface IDentalAppointmentService
    {
        Task<DentalAppointment> CreateAppointmentAsync(DentalAppointmentDto appointmentDto);
        Task<DentalAppointment> UpdateAppointmentAsync(int id, DentalAppointmentDto appointmentDto);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<DentalAppointmentResponseDto> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<DentalAppointmentResponseDto>> GetAppointmentsByDentistIdAsync(string dentistId);
        Task<IEnumerable<DentalAppointmentResponseDto>> GetAppointmentsByPatientIdAsync(string patientId);
    }
}
