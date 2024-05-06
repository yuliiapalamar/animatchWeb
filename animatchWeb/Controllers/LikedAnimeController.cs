using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        [HttpPost]
        public async Task<IActionResult> Like(int animeId)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            var likedAnime = new LikedAnime
            {
                UserId = user.Id,
                AnimeId = animeId,
                
            };

            _context.LikedAnime.Add(likedAnime);

            await _context.SaveChangesAsync();

			string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

			// Перенаправляємо користувача на поточну сторінку
			return Redirect(returnUrl);
		}

        [HttpPost]
        public async Task<IActionResult> Unlike(int animeId)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            var likedAnime = await _context.LikedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

            if (likedAnime != null)
            {
                _context.LikedAnime.Remove(likedAnime);
                await _context.SaveChangesAsync();
            }

			string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

			// Перенаправляємо користувача на поточну сторінку
			return Redirect(returnUrl);
		}

        public async Task<bool> IsLiked(int animeId, string email)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == email);
            var likedAnime = await _context.LikedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

            if (likedAnime != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
