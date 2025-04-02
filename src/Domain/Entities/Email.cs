using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmileTimeNET_API.src.Domain.Entities
{
    [Table("Emails")]
    public class Email
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Token { get; set; } = string.Empty;

        public DateTime FechaExpiracion { get; set; }

        [Required]
        public bool Usado { get; set; } = false;
    }
}