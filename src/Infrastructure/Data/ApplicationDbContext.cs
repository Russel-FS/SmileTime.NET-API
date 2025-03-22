using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SmileTimeNET_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SmileTimeNET_API.src.Domain.Models;
using System.Reflection.Emit;


namespace SmileTimeNET_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        public DbSet<Carousels> Carousels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Puedes añadir configuración adicional para la entidad Carousel aquí
            builder.Entity<Carousels>()
                .HasIndex(c => c.Titulo);
        }
    }
}