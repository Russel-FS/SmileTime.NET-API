namespace SmileTimeNET.Infrastructure.Api.Dentist
{
    public class DentalAppointmentDto
    {
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string? Notes { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public PatientInfoDto? PatientInfo { get; set; }
    }

    public class PatientInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
