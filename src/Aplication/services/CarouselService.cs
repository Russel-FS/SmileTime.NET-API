using Microsoft.EntityFrameworkCore;
using SmileTimeNET_API.Data;
using SmileTimeNET_API.src.Domain.Models;

namespace SmileTimeNET_API.src.Aplication.services
{
    public class CarouselService
    {
        private readonly ApplicationDbContext _context;

        public CarouselService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Carousels>> GetAllCarouselsAsync()
        {
            return await _context.Carousels
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Carousels>> GetActiveCarouselsAsync()
        {
            return await _context.Carousels
                .Where(c => c.Activo)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<Carousels> GetCarouselByIdAsync(int id)
        {
            return await _context.Carousels
                .FirstOrDefaultAsync(c => c.Id == id) 
                ?? throw new InvalidOperationException($"Carousel with ID {id} not found.");
        }

        public async Task<Carousels> CreateCarouselAsync(Carousels carousel)
        {
            _context.Carousels.Add(carousel);
            await _context.SaveChangesAsync();
            return carousel;
        }

        public async Task<Carousels?> UpdateCarouselAsync(Carousels carousel)
        {
            var existingCarousel = await _context.Carousels.FindAsync(carousel.Id);

            if (existingCarousel == null)
                return null;

            _context.Entry(existingCarousel).CurrentValues.SetValues(carousel);
            await _context.SaveChangesAsync();

            return existingCarousel;
        }

        public async Task<bool> DeleteCarouselAsync(int id)
        {
            var carousel = await _context.Carousels.FindAsync(id);

            if (carousel == null)
                return false;

            _context.Carousels.Remove(carousel);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleCarouselStatusAsync(int id)
        {
            var carousel = await _context.Carousels.FindAsync(id);

            if (carousel == null)
                return false;

            carousel.Activo = !carousel.Activo;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
