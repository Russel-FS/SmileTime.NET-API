using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmileTimeNET_API.src.Domain.Models
{
    [Table("Carousels")]
    public class Carousels
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string? ImagenUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string? Titulo { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(255)]
        public string? Alt { get; set; }

        [Required]
        public bool Activo { get; set; } = true;
    }
}