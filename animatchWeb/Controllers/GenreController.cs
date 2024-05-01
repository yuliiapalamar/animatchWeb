using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace animatchWeb.Controllers
{
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genre>> GetAllReviews()
        {
            var reviewList = await _context.Genre.ToListAsync();
            return reviewList;
        }

        public async Task<List<string>> GetGenresForAnime(int id)
        {
            var genres = await _context.AnimeGenre
                .Where(ag => ag.AnimeId == id)
                .Join(_context.Genre, ag => ag.GenreId, g => g.Id, (ag, g) => g.Name)
                .ToListAsync();

            return genres;
        }

    }
}
