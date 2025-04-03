using Microsoft.AspNetCore.Identity;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;
using SmileTimeNET_API.Domain.Entities.Dentist;

namespace SmileTimeNET_API.src.Aplication.services.dentist
{
    public class PacientManagementServiceImpl : IPacientManagementService
    {
        private readonly string PACIENT_ROLE = "User";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService;

        public PacientManagementServiceImpl(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authService = authService;
        }

        public async Task<ApplicationUser> GetPacientByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !await _userManager.IsInRoleAsync(user, PACIENT_ROLE))
                return null;
            return user;
        }

        /// <summary>
        /// Obtiene todos los usuarios que tienen el rol de 'Pacient'.
        /// </summary>
        /// <returns>
        /// Una colección de <see cref="ApplicationUser"/> que representa a todos los pacientes.
        /// </returns>
        public async Task<IEnumerable<ApplicationUser>> GetAllPacientsAsync()
        {
            return await _userManager.GetUsersInRoleAsync(PACIENT_ROLE);
        }

        /// <summary>
        /// Elimina un paciente de la base de datos.
        /// </summary>
        /// <param name="id">El ID del paciente a eliminar.</param>
        /// <returns>
        /// <see langword="true"/> si el paciente fue eliminado exitosamente; de lo contrario, <see langword="false"/>.

        public async Task<bool> UpdatePacientAsync(string id, ApplicationUser updatedPacient)
        {
            var pacient = await _userManager.FindByIdAsync(id);
            if (pacient == null || !await _userManager.IsInRoleAsync(pacient, PACIENT_ROLE))
                return false;

            pacient.UserName = updatedPacient.UserName;
            pacient.Email = updatedPacient.Email;
            pacient.PhoneNumber = updatedPacient.PhoneNumber;
            var result = await _userManager.UpdateAsync(pacient);
            return result.Succeeded;
        }
        /// <summary>
        /// Elimina un paciente de la base de datos.
        /// </summary>
        /// <param name="id">El ID del paciente a eliminar.</param>
        /// <returns>
        /// <see langword="true"/> si el paciente fue eliminado exitosamente; de lo contrario, <see langword="false"/>.
        public async Task<bool> DeletePacientAsync(string id)
        {
            var pacient = await _userManager.FindByIdAsync(id);
            if (pacient == null || !await _userManager.IsInRoleAsync(pacient, PACIENT_ROLE))
                return false;

            var result = await _userManager.DeleteAsync(pacient);
            return result.Succeeded;
        }

        /// <summary>
        /// Asigna el rol de 'Pacient' a un usuario, creando el rol si no existe.
        /// </summary>
        /// <param name="userId">El ID del usuario al que se le asignará el rol.</param>
        /// <returns>
        /// <see langword="true"/> si el rol fue asignado exitosamente o si el usuario ya tenía el rol; de lo contrario, <see langword="false"/>.
        /// </returns>
        public async Task<bool> AssignPacientRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (!await _roleManager.RoleExistsAsync(PACIENT_ROLE))
                await _roleManager.CreateAsync(new IdentityRole(PACIENT_ROLE));

            if (!await _userManager.IsInRoleAsync(user, PACIENT_ROLE))
            {
                var result = await _userManager.AddToRoleAsync(user, PACIENT_ROLE);
                return result.Succeeded;
            }

            return true;
        }

        /// <summary>
        /// Crea un nuevo usuario de aplicación basado en el modelo de paciente proporcionado 
        /// y le asigna el rol de 'Pacient'.
        /// </summary>
        /// <param name="model">El modelo de paciente que contiene los datos necesarios para crear un usuario.</param>
        /// <returns><see langword="true"/> si el paciente fue creado y el rol fue asignado exitosamente; de lo contrario, <see langword="false"/>.</returns>
        public async Task<bool> CreatePacientFromUserAsync(PacientModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.Phone,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) return false;

            return await AssignPacientRoleAsync(user.Id);
        }


        /// <summary>
        /// Elimina el rol de 'Pacient' de un usuario.
        /// </summary>
        /// <param name="userId"> El ID del usuario.</param>
        /// <returns>
        /// <see langword="true"/>  si el rol fue eliminado exitosamente; de lo contrario, <see langword="false"/>.
        /// </returns>
        public async Task<bool> RemovePacientRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return (await _userManager.RemoveFromRoleAsync(user, PACIENT_ROLE)).Succeeded;
        }
        /// <summary>
        /// Busca pacientes por su nombre o username.
        /// </summary>
        /// <param name="searchTerm">El t rmino de b squeda.</param>
        /// <returns>
        /// Una colecci n de <see cref="PacientModel"/> que representa los pacientes que coinciden con el t rmino de b squeda.
        /// </returns>
        public async Task<IEnumerable<PacientModel>> SearchPacientsByNameAsync(string searchTerm)
        {
            var allPacients = await GetAllPacientsAsync();
            return allPacients
                .Where(p =>
                    (p.FullName != null && p.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (p.UserName != null && p.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .Select(p => new PacientModel
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    Email = p.Email ?? string.Empty,
                    Phone = p.PhoneNumber,
                    Name = p.UserName
                });
        }
    }
}
