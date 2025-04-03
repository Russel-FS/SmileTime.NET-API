using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileTimeNET.Domain.Entities.Dentist
{
    public class DentalAppointment
    {
        [Key]
        public int Id { get; set; }
        
        public string PatientId { get; set; } = string.Empty;
        
        public string DentistId { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public string Time { get; set; } = string.Empty;
        
        [Required]
        public AppointmentType Type { get; set; }
        
        [Required]
        public AppointmentStatus Status { get; set; }
        
        [Required]
        public int Duration { get; set; }
        
        public string Notes { get; set; } = string.Empty;
    }
}
