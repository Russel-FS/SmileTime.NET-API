using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmileTimeNET_API.src.Aplication.services;
using SmileTimeNET_API.src.Domain.Models;

namespace SmileTimeNET_API.src.Infrastructure.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarouselController : ControllerBase
    {
        private readonly CarouselService _carouselService;

        public CarouselController(CarouselService carouselService)
        {
            _carouselService = carouselService;
        }

        // GET: api/Carousels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carousels>>> GetCarousels()
        {
            var carousels = await _carouselService.GetAllCarouselsAsync();
            return Ok(carousels);
        }

        // GET: api/Carousels/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Carousels>>> GetActiveCarousels()
        {
            var carousels = await _carouselService.GetActiveCarouselsAsync();
            return Ok(carousels);
        }

        // GET: api/Carousels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Carousels>> GetCarousel(int id)
        {
            var carousel = await _carouselService.GetCarouselByIdAsync(id);

            if (carousel == null)
            {
                return NotFound();
            }

            return carousel;
        }

        // POST: api/Carousels
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Carousels>> CreateCarousel(Carousels carousel)
        {
            try
            {
                var createdCarousel = await _carouselService.CreateCarouselAsync(carousel);
                return CreatedAtAction(nameof(GetCarousel), new { id = createdCarousel.Id }, createdCarousel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Carousels/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCarousel(int id, Carousels carousel)
        {
            if (id != carousel.Id)
            {
                return BadRequest();
            }

            var updatedCarousel = await _carouselService.UpdateCarouselAsync(carousel);

            if (updatedCarousel == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/Carousels/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCarousel(int id)
        {
            var result = await _carouselService.DeleteCarouselAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // PATCH: api/Carousels/toggle/5
        [HttpPatch("toggle/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleCarouselStatus(int id)
        {
            var result = await _carouselService.ToggleCarouselStatusAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
