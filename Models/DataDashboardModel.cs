using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class DataDashboardModel
    {
        public int TotalPatients { get; set; }
        public int NewPatients { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedTreatments { get; set; }
        public required Dictionary<string, int> MonthlyAppointments { get; set; }
        public required TreatmentStats Treatments { get; set; }
        public required Metrics Metrics { get; set; }
    }

    public class TreatmentStats
    {
        public int Cleanings { get; set; }
        public int Extractions { get; set; }
        public int Fillings { get; set; }
    }

    public class Metrics
    {
        public int TotalPatientsChange { get; set; }
        public int NewPatientsChange { get; set; }
        public int PendingAppointmentsChange { get; set; }
        public int CompletedTreatmentsChange { get; set; }
    }
}
