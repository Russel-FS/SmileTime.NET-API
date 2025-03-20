using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.Models;
using System.Collections.Generic;

namespace SmileTimeNET_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        // Simulación de datos del dashboard
        private static readonly DataDashboardModel DashboardData = new DataDashboardModel
        {
            TotalPatients = 100,
            NewPatients = 10,
            PendingAppointments = 20,
            CompletedTreatments = 30,
            MonthlyAppointments = new Dictionary<string, int>
            {
                { "January", 10 },
                { "February", 15 },
                { "March", 20 }
            },
            Treatments = new TreatmentStats
            {
                Cleanings = 10,
                Extractions = 5,
                Fillings = 15
            },
            Metrics = new Metrics
            {
                TotalPatientsChange = 5,
                NewPatientsChange = 2,
                PendingAppointmentsChange = -1,
                CompletedTreatmentsChange = 3
            }
        };

        [HttpGet]
        public ActionResult<DataDashboardModel> GetDashboardData()
        {
            return Ok(DashboardData);
        }

        [HttpPost]
        public ActionResult UpdateDashboardData([FromBody] DataDashboardModel updatedData)
        {
            if (updatedData == null)
            {
                return BadRequest("Updated data cannot be null.");
            }
            // Aquí puedes agregar lógica para actualizar los datos del dashboard
            return NoContent();
        }
    }
}
