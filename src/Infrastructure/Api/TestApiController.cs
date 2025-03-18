using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmileTimeNET_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    [Authorize]
    public class TestApiController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("Prueba exitosa");
        }
    }
}