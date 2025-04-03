namespace SmileTimeNET.Domain.Entities.Dentist
{
    public enum AppointmentType
    {
        Limpieza,
        Consulta,
        Tratamiento
    }

    public enum AppointmentStatus
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public enum PatientStatus
    {
        Active,
        Pending,
        Inactive
    }
}
