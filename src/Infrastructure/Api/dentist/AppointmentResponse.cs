namespace SmileTimeNET.Infrastructure.Api.Dentist
{
    public class AppointmentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AppointmentData? Data { get; set; }
    }

    public class AppointmentData
    {
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public PatientInfo? PatientInfo { get; set; }
    }

    public class PatientInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
