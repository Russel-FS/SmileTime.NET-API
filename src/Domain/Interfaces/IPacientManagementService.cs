using SmileTimeNET_API.Domain.Entities.Dentist;
using SmileTimeNET_API.Models;

namespace SmileTimeNET_API.src.Domain.Interfaces
{
    public interface IPacientManagementService
    {
        Task<ApplicationUser> GetPacientByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllPacientsAsync();
        Task<bool> UpdatePacientAsync(string id, ApplicationUser updatedPacient);
        Task<bool> DeletePacientAsync(string id);
        Task<bool> AssignPacientRoleAsync(string userId);
        Task<bool> CreatePacientFromUserAsync(PacientModel model);
        Task<bool> RemovePacientRoleAsync(string userId);
        Task<IEnumerable<PacientModel>> SearchPacientsByNameAsync(string searchTerm);
    }
}
