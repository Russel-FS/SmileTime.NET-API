using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
            },
            FrequentPatients = new List<FrequentPatient>
            {
                new FrequentPatient { Name = "Juan Pérez", Treatment = "Limpieza Dental", Appointments = 3 },
                new FrequentPatient { Name = "Manuel Ríos", Treatment = "Implante Dental", Appointments = 8 },
                new FrequentPatient { Name = "Laura Torres", Treatment = "Blanqueamiento", Appointments = 6 },
                new FrequentPatient { Name = "Carlos Ruiz", Treatment = "Empaste", Appointments = 2 },
                new FrequentPatient { Name = "Ana García", Treatment = "Limpieza Dental", Appointments = 10 }
            },
            UpcomingAppointments = new List<Appointment>
            {
                new Appointment { Patient = "Juan Pérez", Doctor = "Dr. García", Date = "2024-01-20", Time = "09:00", Treatment = "Limpieza Dental" },
                new Appointment { Patient = "María López", Doctor = "Dr. Rodríguez", Date = "2024-01-20", Time = "10:30", Treatment = "Extracción" },
                new Appointment { Patient = "Carlos Ruiz", Doctor = "Dr. García", Date = "2024-01-21", Time = "11:00", Treatment = "Empaste" },
                new Appointment { Patient = "Velazquez Navarro", Doctor = "Dr. Malca Tucto", Date = "2025-03-05", Time = "11:00", Treatment = "Revisión ortodoncia" },
                new Appointment { Patient = "Mariana Pérez", Doctor = "Dr. Flores Solano", Date = "2025-03-05", Time = "12:00", Treatment = "Empaste" }
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

        [HttpGet("frequent-patients")]
        public ActionResult<IEnumerable<FrequentPatient>> GetFrequentPatients()
        {
            var frequentPatients = new List<FrequentPatient>
            {
                new FrequentPatient { Name = "Juan Pérez", Treatment = "Limpieza Dental", Appointments = 3 },
                new FrequentPatient { Name = "María López", Treatment = "Extracción", Appointments = 2 }
            };
            return Ok(frequentPatients);
        }

        [HttpGet("upcoming-appointments")]
        public ActionResult<IEnumerable<Appointment>> GetUpcomingAppointments()
        {
            var upcomingAppointments = new List<Appointment>
            {
                new Appointment { Patient = "Juan Pérez", Doctor = "Dr. García", Date = "2024-01-20", Time = "09:00", Treatment = "Limpieza Dental" },
                new Appointment { Patient = "María López", Doctor = "Dr. Rodríguez", Date = "2024-01-20", Time = "10:30", Treatment = "Extracción" }
            };
            return Ok(upcomingAppointments);
        }

        [HttpGet("filter-by-date")]
        public ActionResult<DataDashboardModel> GetDashboardDataByDateRange([FromQuery] string range)
        {
            DateTime now = DateTime.Now;
            DateTime startDate;

            switch (range.ToLower())
            {
                case "today":
                    startDate = now.Date;
                    break;
                case "week":
                    startDate = now.Date.AddDays(-(int)now.DayOfWeek);
                    break;
                case "month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
                case "year":
                    startDate = new DateTime(now.Year, 1, 1);
                    break;
                default:
                    return BadRequest("Invalid range. Use 'today', 'week', 'month', or 'year'.");
            }

            var filteredAppointments = DashboardData.UpcomingAppointments
                .Where(a =>
                {
                    if (DateTime.TryParse(a.Date, out var appointmentDate))
                    {
                        return appointmentDate >= startDate;
                    }
                    return false;
                })
                .ToList();

            var filteredDashboardData = new DataDashboardModel
            {
                TotalPatients = DashboardData.TotalPatients,
                NewPatients = DashboardData.NewPatients,
                PendingAppointments = filteredAppointments.Count,
                CompletedTreatments = DashboardData.CompletedTreatments,
                MonthlyAppointments = DashboardData.MonthlyAppointments,
                Treatments = DashboardData.Treatments,
                Metrics = DashboardData.Metrics,
                FrequentPatients = DashboardData.FrequentPatients,
                UpcomingAppointments = filteredAppointments
            };

            return Ok(filteredDashboardData);
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<object> GetAdditionalStatistics()
        {
            var statistics = new
            {
                RevenueGenerated = 50000, // Ejemplo de ingresos generados
                MostPopularTreatments = new Dictionary<string, int>
                {
                    { "Limpieza Dental", 50 },
                    { "Empaste", 30 },
                    { "Extracción", 20 }
                }
            };
            return Ok(statistics);
        }

        [HttpGet("export-pdf")]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportDashboardToPdf()
        {
            // Simulación de exportación a PDF
            var pdfContent = "Datos del Dashboard en formato PDF"; // Aquí se generaría el contenido real del PDF
            var pdfBytes = System.Text.Encoding.UTF8.GetBytes(pdfContent);

            return File(pdfBytes, "application/pdf", "Dashboard.pdf");
        }
    }
}
