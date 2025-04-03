namespace SmileTimeNET.Domain.Entities.Dentist
{
    public enum AppointmentType
    {
        Regular,
        Emergency,
        Followup,
        Cleaning
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled,
        NoShow
    }

    public enum PatientStatus
    {
        Active,
        Inactive,
        Pending
    }
}
