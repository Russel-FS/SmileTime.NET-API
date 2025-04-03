using Microsoft.AspNetCore.Identity;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class DentistManagementServiceImpl : IDentistManagementService
    {
        private readonly string DENTIST_ROLE = "Dentist";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService;

        public DentistManagementServiceImpl(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authService = authService;
        }

        public async Task<ApplicationUser> GetDentistByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || !await _userManager.IsInRoleAsync(user, DENTIST_ROLE))
                return null;
            return user;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllDentistsAsync()
        {
            return await _userManager.GetUsersInRoleAsync(DENTIST_ROLE);
        }

        public async Task<bool> UpdateDentistAsync(string id, ApplicationUser updatedDentist)
        {
            var dentist = await _userManager.FindByIdAsync(id);
            if (dentist == null || !await _userManager.IsInRoleAsync(dentist, DENTIST_ROLE))
                return false;

            dentist.UserName = updatedDentist.UserName;
            dentist.Email = updatedDentist.Email;
            var result = await _userManager.UpdateAsync(dentist);
            return result.Succeeded;
        }

        public async Task<bool> DeleteDentistAsync(string id)
        {
            var dentist = await _userManager.FindByIdAsync(id);
            if (dentist == null || !await _userManager.IsInRoleAsync(dentist, DENTIST_ROLE))
                return false;

            var result = await _userManager.DeleteAsync(dentist);
            return result.Succeeded;
        }

        public async Task<bool> AssignDentistRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (!await _roleManager.RoleExistsAsync(DENTIST_ROLE))
                await _roleManager.CreateAsync(new IdentityRole(DENTIST_ROLE));

            if (!await _userManager.IsInRoleAsync(user, DENTIST_ROLE))
            {
                var result = await _userManager.AddToRoleAsync(user, DENTIST_ROLE);
                return result.Succeeded;
            }

            return true;
        }

        public async Task<bool> CreateDentistFromUserAsync(RegisterModel model)
        { 
            var authResponse = await _authService.RegisterAsync(model);
            
            if (!authResponse.Success)
                return false;

            // Asignar rol de dentista
            return await AssignDentistRoleAsync(authResponse.UserId);
        }

        public async Task<bool> RemoveDentistRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return (await _userManager.RemoveFromRoleAsync(user, DENTIST_ROLE)).Succeeded;
        }
    }
}
