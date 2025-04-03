using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.src.Infrastructure.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminManagementController : ControllerBase
    {
        private readonly IAdminManagementService _adminService;

        public AdminManagementController(IAdminManagementService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            return Ok(admins);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterModel model)
        {
            try
            {
                var result = await _adminService.CreateAdminFromUserAsync(model);
                if (!result)
                    return BadRequest("No se pudo crear el administrador");
                return Ok("Administrador creado exitosamente");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("assign/{userId}")]
        public async Task<IActionResult> AssignAdminRole(string userId)
        {
            try
            {
                var result = await _adminService.AssignAdminRoleAsync(userId);
                if (!result)
                    return BadRequest("No se pudo asignar el rol de administrador");
                return Ok("Rol de administrador asignado exitosamente");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }

        [HttpDelete("remove-role/{userId}")]
        public async Task<IActionResult> RemoveAdminRole(string userId)
        {
            var result = await _adminService.RemoveAdminRoleAsync(userId);
            if (!result)
                return BadRequest("No se pudo remover el rol de administrador");
            return Ok("Rol de administrador removido exitosamente");
        }
    }
}
