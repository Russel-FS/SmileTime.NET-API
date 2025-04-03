using System.ComponentModel;

namespace SmileTimeNET.Domain.Entities.Dentist
{
    public enum AppointmentType
    {
        [Description("limpieza")]
        limpieza = 0,
        [Description("consulta")]
        consulta = 1,
        [Description("tratamiento")]
        tratamiento = 2
    }

    public enum AppointmentStatus
    {
        [Description("pendiente")]
        pendiente = 0,
        [Description("confirmada")]
        confirmada = 1,
        [Description("cancelada")]
        cancelada = 2
    }

    public enum PatientStatus
    {
        [Description("active")]
        active = 0,
        [Description("pending")]
        pending = 1,
        [Description("inactive")]
        inactive = 2
    }
}
