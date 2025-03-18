using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SmileTimeNET_API.src.Infrastructure.Data.Seeds
{
    public static class RoleSeeder
    {
        /// <summary>
        /// Sembrar roles en la base de datos.
        /// </summary>
        /// <param name="serviceProvider">El proveedor de servicios.</param>
        /// <returns>
        /// Una tarea asíncrona que representa la semilla de roles en la base de datos.
        /// </returns>
        /// <remarks>
        /// El metodo asincrono SeedRolesAsync se utiliza para sembrar roles en la base de datos.
        /// </remarks>
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User", "Odontologo" };

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

    }
}