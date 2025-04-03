using Microsoft.AspNetCore.Identity;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class AdminManagementServiceImpl : IAdminManagementService
    {
        private readonly string ADMIN_ROLE = "Admin";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthService _authService;

        public AdminManagementServiceImpl(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authService = authService;
        }

        public async Task<bool> AssignAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (!await _roleManager.RoleExistsAsync(ADMIN_ROLE))
                await _roleManager.CreateAsync(new IdentityRole(ADMIN_ROLE));

            return (await _userManager.AddToRoleAsync(user, ADMIN_ROLE)).Succeeded;
        }

        public async Task<bool> RemoveAdminRoleAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            return (await _userManager.RemoveFromRoleAsync(user, ADMIN_ROLE)).Succeeded;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAdminsAsync()
        {
            return await _userManager.GetUsersInRoleAsync(ADMIN_ROLE);
        }

        public async Task<bool> CreateAdminFromUserAsync(RegisterModel model)
        {
            var authResponse = await _authService.RegisterAsync(model);
            
            if (!authResponse.Success)
                return false;

            return await AssignAdminRoleAsync(authResponse.UserId);
        }
    }
}
