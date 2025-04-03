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

        public async Task<IEnumerable<ApplicationUser>> GetAllPacientsAsync()
        {
            return await _userManager.GetUsersInRoleAsync(PACIENT_ROLE);
        }

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

        public async Task<bool> DeletePacientAsync(string id)
        {
            var pacient = await _userManager.FindByIdAsync(id);
            if (pacient == null || !await _userManager.IsInRoleAsync(pacient, PACIENT_ROLE))
                return false;

            var result = await _userManager.DeleteAsync(pacient);
            return result.Succeeded;
        }

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

        public async Task<bool> RemovePacientRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return (await _userManager.RemoveFromRoleAsync(user, PACIENT_ROLE)).Succeeded;
        }

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
