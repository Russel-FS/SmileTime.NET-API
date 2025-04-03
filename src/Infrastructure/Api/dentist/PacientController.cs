using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.Domain.Entities.Dentist;
using SmileTimeNET_API.Models;
using SmileTimeNET_API.src.Domain.Interfaces;

namespace SmileTimeNET_API.Infrastructure.Api.Dentist
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PacientController : ControllerBase
    {
        private readonly IPacientManagementService _pacientService;

        public PacientController(IPacientManagementService pacientService)
        {
            _pacientService = pacientService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAllPacients()
        {
            var pacients = await _pacientService.GetAllPacientsAsync();
            return Ok(pacients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetPacientById(string id)
        {
            var pacient = await _pacientService.GetPacientByIdAsync(id);
            if (pacient == null)
                return NotFound();
            return Ok(pacient);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PacientModel>>> SearchPacients([FromQuery] string searchTerm)
        {
            var pacients = await _pacientService.SearchPacientsByNameAsync(searchTerm);
            return Ok(pacients);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePacient([FromBody] PacientModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _pacientService.CreatePacientFromUserAsync(model);
            if (!result)
                return BadRequest("No se pudo crear el paciente");

            return Ok("Paciente creado exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePacient(string id, [FromBody] ApplicationUser updatedPacient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _pacientService.UpdatePacientAsync(id, updatedPacient);
            if (!result)
                return NotFound();

            return Ok("Paciente actualizado exitosamente");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePacient(string id)
        {
            var result = await _pacientService.DeletePacientAsync(id);
            if (!result)
                return NotFound();

            return Ok("Paciente eliminado exitosamente");
        }

        [HttpPost("{userId}/assign-role")]
        public async Task<IActionResult> AssignPacientRole(string userId)
        {
            var result = await _pacientService.AssignPacientRoleAsync(userId);
            if (!result)
                return BadRequest("No se pudo asignar el rol de paciente");

            return Ok("Rol de paciente asignado exitosamente");
        }

        [HttpPost("{userId}/remove-role")]
        public async Task<IActionResult> RemovePacientRole(string userId)
        {
            var result = await _pacientService.RemovePacientRoleAsync(userId);
            if (!result)
                return BadRequest("No se pudo remover el rol de paciente");

            return Ok("Rol de paciente removido exitosamente");
        }
    }
}
