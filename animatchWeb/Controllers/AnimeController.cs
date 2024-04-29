using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animatchWeb.Data;
using animatchWeb.Models;

namespace animatchWeb.Controllers
{
    public class AnimeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Метод для отримання списку всіх аніме
        public async Task<List<Anime>> GetAllAnime(string SearchString)
        {
            var animeList = await _context.Anime.ToListAsync();
            if (!string.IsNullOrEmpty(SearchString))
            {
                animeList = animeList.Where(a => a.Name.ToLower().Contains(SearchString.ToLower())).ToList();
            }
            return animeList;
        }


        // Метод для отримання інформації про конкретне аніме за його Id
        public async Task<IActionResult> Details(int id)
        {
            var anime = await _context.Anime.FirstOrDefaultAsync(a => a.Id == id);
            if (anime == null)
            {
                return NotFound();
            }

            var reviews = await _context.Review.Where(r => r.AnimeId == id).ToListAsync();

            var model = new Tuple<Anime, List<Review>>(anime, reviews);

            return View(model);
        }


        // Метод для отримання випадкового аніме зі списку
        public async Task<IActionResult> GetRandomAnime()
        {
            var randomAnime = await _context.Anime.OrderBy(a => Guid.NewGuid()).FirstOrDefaultAsync();
            return View(randomAnime);
        }

    }
}