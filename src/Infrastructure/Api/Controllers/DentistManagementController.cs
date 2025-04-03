using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Infrastructure.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DentistManagementController : ControllerBase
    {
        private readonly IDentistManagementService _dentistService;

        public DentistManagementController(IDentistManagementService dentistService)
        {
            _dentistService = dentistService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllDentists()
        {
            var dentists = await _dentistService.GetAllDentistsAsync();
            return Ok(dentists);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetDentist(string id)
        {
            var dentist = await _dentistService.GetDentistByIdAsync(id);
            if (dentist == null)
                return NotFound();
            return Ok(dentist);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDentist([FromBody] RegisterModel model)
        {
            var result = await _dentistService.CreateDentistFromUserAsync(model);
            if (!result)
                return BadRequest("No se pudo crear el dentista");
            return Ok("Dentista creado exitosamente");
        }

        [HttpPost("assign/{userId}")]
        public async Task<IActionResult> AssignDentistRole(string userId)
        {
            var result = await _dentistService.AssignDentistRoleAsync(userId);
            if (!result)
                return BadRequest("No se pudo asignar el rol de dentista");
            return Ok("Rol de dentista asignado exitosamente");
        }

        [HttpDelete("remove-role/{userId}")]
        public async Task<IActionResult> RemoveDentistRole(string userId)
        {
            var result = await _dentistService.RemoveDentistRoleAsync(userId);
            if (!result)
                return BadRequest("No se pudo remover el rol de dentista");
            return Ok("Rol de dentista removido exitosamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDentist(string id)
        {
            var result = await _dentistService.DeleteDentistAsync(id);
            if (!result)
                return BadRequest("No se pudo eliminar el dentista");
            return Ok("Dentista eliminado exitosamente");
        }
    }
}
