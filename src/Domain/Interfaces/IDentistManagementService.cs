using SmileTimeNET_API.Models;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IDentistManagementService
    {
        Task<ApplicationUser> GetDentistByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllDentistsAsync();
        Task<bool> UpdateDentistAsync(string id, ApplicationUser dentist);
        Task<bool> DeleteDentistAsync(string id);
        Task<bool> AssignDentistRoleAsync(string userId);
        Task<bool> CreateDentistFromUserAsync(RegisterModel model);
        Task<bool> RemoveDentistRoleAsync(string userId);
    }
}
