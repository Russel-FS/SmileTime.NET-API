namespace SmileTimeNET.Infrastructure.Api.Dentist
{
    public class DentalAppointmentResponseDto
    {
        public AppointmentDto Appointment { get; set; } = new();
        public PatientInfoDto PatientInfo { get; set; } = new();
    }

    public class AppointmentDto
    {
        public string PatientId { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
