using SmileTimeNET_API.Models;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IAdminManagementService
    {
        Task<bool> AssignAdminRoleAsync(string userId);
        Task<bool> RemoveAdminRoleAsync(string userId);
        Task<IEnumerable<ApplicationUser>> GetAllAdminsAsync();
        Task<bool> CreateAdminFromUserAsync(RegisterModel model);
    }
}
