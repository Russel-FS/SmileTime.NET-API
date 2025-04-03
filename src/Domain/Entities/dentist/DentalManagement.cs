using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmileTimeNET.Domain.Entities.Dentist
{
    public class DentalManagement
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
  
        public string? Phone { get; set; }
        
        [Required]
        public PatientStatus Status { get; set; }
        
        public DateTime? LastVisit { get; set; }
        
        public virtual ICollection<DentalAppointment> Appointments { get; set; }

        public DentalManagement()
        {
            Appointments = new HashSet<DentalAppointment>();
        }
    }
}
