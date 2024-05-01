using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace animatchWeb.Controllers
{
    public class AddedAnimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddedAnimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Save(int animeId)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            var likedAnime = new AddedAnime
            {
                UserId = user.Id,
                AnimeId = animeId,

            };

            _context.AddedAnime.Add(likedAnime);

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Home", new { id = animeId });
        }

        [HttpPost]
        public async Task<IActionResult> UnSave(int animeId)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

            var likedAnime = await _context.AddedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

            if (likedAnime != null)
            {
                _context.AddedAnime.Remove(likedAnime);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Home", new { id = animeId });
        }

        public async Task<bool> IsAdded(int animeId, string email)
        {
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == email);
            var likedAnime = await _context.AddedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

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
