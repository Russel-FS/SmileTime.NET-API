using SmileTimeNET.Domain.Entities.Dentist;
using SmileTimeNET.Infrastructure.Api.Dentist;

namespace SmileTimeNET.Application.Services.Dentist
{
    public interface IDentalAppointmentService
    {
        Task<DentalAppointment> CreateAppointmentAsync(DentalAppointmentDto appointmentDto);
        Task<DentalAppointment> UpdateAppointmentAsync(int id, DentalAppointmentDto appointmentDto);
        Task<bool> DeleteAppointmentAsync(int id);
        Task<DentalAppointment> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<DentalAppointment>> GetAppointmentsByDentistIdAsync(string dentistId);
        Task<IEnumerable<DentalAppointment>> GetAppointmentsByPatientIdAsync(string patientId);
    }
}
