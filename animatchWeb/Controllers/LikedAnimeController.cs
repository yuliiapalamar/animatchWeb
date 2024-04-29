using animatchWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace animatchWeb.Controllers
{
    public class LikedAnimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LikedAnimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> LikedList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var likedAnime = await _context.LikedAnime
                .Where(m => m.UserId == id)
                .Join(_context.Anime, a => a.AnimeId, g => g.Id, (a, g) => g)
                .ToListAsync();

            return View(likedAnime);
        }
    }
}
