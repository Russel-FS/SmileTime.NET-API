using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmileTimeNET_API.src.Domain.Models.auth
{
    public class EmailModel
    {
        [Required]
        [EmailAddress]

        public string? Email { get; set; }
    }
}