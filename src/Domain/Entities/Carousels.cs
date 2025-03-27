using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SmileTimeNET_API.src.Domain.Models
{
    [Table("Carousels")]
    public class Carousels
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string ImagenUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(255)]
        public string Alt { get; set; } = string.Empty;

        [Required]
        public bool Activo { get; set; } = true;
    }
}