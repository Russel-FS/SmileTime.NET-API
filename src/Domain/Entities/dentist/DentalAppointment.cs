using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmileTimeNET_API.Models;

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
        public string Type { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = string.Empty;
        
        [Required]
        public int Duration { get; set; }
        
        public string Notes { get; set; } = string.Empty;
        
        [ForeignKey("PatientId")]
        public virtual ApplicationUser Patient { get; set; }
        
        [ForeignKey("DentistId")]
        public virtual ApplicationUser Dentist { get; set; }
    }
}
