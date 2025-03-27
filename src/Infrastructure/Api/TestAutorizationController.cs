using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmileTimeNET_API.src.Infrastructure.Api
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TestAutorizationController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Autorizado");
        }
    }
}