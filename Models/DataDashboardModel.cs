using System;
using System.Collections.Generic;

namespace SmileTimeNET_API.Models
{
    public class DataDashboardModel
    {
        public int TotalPatients { get; set; }
        public int NewPatients { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedTreatments { get; set; }
        public Dictionary<string, int> MonthlyAppointments { get; set; } = new();
        public TreatmentStats Treatments { get; set; } = new();
        public Metrics Metrics { get; set; } = new();
        public List<FrequentPatient> FrequentPatients { get; set; } = new();
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public decimal RevenueGenerated { get; set; }
        public List<string> MostPopularTreatments { get; set; } = new(); // Removed 'required' for compatibility
        public List<Notification> Notifications { get; set; } = new();
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

    public class FrequentPatient
    {
        public string Name { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
        public int Appointments { get; set; }
    }

    public class Appointment
    {
        public string Patient { get; set; } = string.Empty;
        public string Doctor { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }

    public class Notification
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
    }
}
