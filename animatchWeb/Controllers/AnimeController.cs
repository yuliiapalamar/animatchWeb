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
        public async Task<List<Anime>> GetAllAnime(string searchString, List<int> genreIds)
        {
            var animeQuery = _context.Anime.AsQueryable();

            // Фільтрація за назвою аніме
            if (!string.IsNullOrEmpty(searchString))
            {
                animeQuery = animeQuery.Where(a => a.Name.ToLower().Contains(searchString.ToLower()));
            }

            // Фільтрація за жанрами
            if (genreIds != null && genreIds.Any())
            {
                animeQuery = animeQuery
                    .Join(_context.AnimeGenre, a => a.Id, ag => ag.AnimeId, (a, ag) => new { Anime = a, AnimeGenre = ag })
                    .Where(x => genreIds.Contains(x.AnimeGenre.GenreId))
                    .Select(x => x.Anime)
                    .Distinct();

            }

            // Отримати відфільтрований список аніме
            var filteredAnimeList = await animeQuery.ToListAsync();

            return filteredAnimeList;
        }


        // Метод для отримання інформації про конкретне аніме за його Id
        public async Task<Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>> Details(int id)
        {
            var anime = await _context.Anime.FirstOrDefaultAsync(a => a.Id == id);
            if (anime == null)
            {
                return null;
            }

			var reviews = await _context.Review
				.Where(r => r.AnimeId == id)
				.ToListAsync();

			// Отримати інформацію про користувача (UserInfo)
			var userInfos = new List<UserInfo>();
			foreach (var review in reviews)
			{
				var userInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == review.UserId);
				userInfos.Add(userInfo);
			}

			var genres = await _context.AnimeGenre
				.Where(ag => ag.AnimeId == id)
				.Join(_context.Genre, ag => ag.GenreId, g => g.Id, (ag, g) => g)
				.ToListAsync();

            var model = new Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>(anime, reviews, genres, userInfos);

			return model;
        }


        // Метод для отримання випадкового аніме зі списку
        public async Task<Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>> GetRandomAnime()
        {
            var randomAnime = await _context.Anime.OrderBy(a => Guid.NewGuid()).FirstOrDefaultAsync();
            var reviews = await _context.Review
                .Where(r => r.AnimeId == randomAnime.Id)
                .ToListAsync();

            // Отримати інформацію про користувача (UserInfo)
            var userInfos = new List<UserInfo>();
            foreach (var review in reviews)
            {
                var userInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == review.UserId);
                userInfos.Add(userInfo);
            }

            var genres = await _context.AnimeGenre
                .Where(ag => ag.AnimeId == randomAnime.Id)
                .Join(_context.Genre, ag => ag.GenreId, g => g.Id, (ag, g) => g)
                .ToListAsync();

            var model = new Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>(randomAnime, reviews, genres, userInfos);


            return model;
		}

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anime = await _context.Anime.FindAsync(id);
            if (anime == null)
            {
                return NotFound();
            }
            return View(anime);
        }

        // POST: Anime/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Text,Year,Photo,Imdbrate")] Anime anime)
        {
            if (id != anime.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(anime);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimeExists(anime.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("ContentManagement", "Home");
        }

        private bool AnimeExists(int id)
        {
            return _context.Anime.Any(e => e.Id == id);
        }

        public async Task<Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>> getRecommendation(int userId)
        {

            var animeList = _context.Anime
                .Where(a => !_context.LikedAnime.Any(l => l.AnimeId == a.Id && l.UserId == userId) &&
                            !_context.AddedAnime.Any(s => s.AnimeId == a.Id && s.UserId == userId) &&
                            !_context.DislikedAnime.Any(d => d.AnimeId == a.Id && d.UserId == userId) &&
                            !_context.WatchedAnime.Any(w => w.AnimeId == a.Id && w.UserId == userId))
                .ToList();

            var randomAnime = animeList.OrderBy(a => Guid.NewGuid()).FirstOrDefault();

            var reviews = await _context.Review
                .Where(r => r.AnimeId == randomAnime.Id)
                .ToListAsync();

            // Отримати інформацію про користувача (UserInfo)
            var userInfos = new List<UserInfo>();
            foreach (var review in reviews)
            {
                var userInfo = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == review.UserId);
                userInfos.Add(userInfo);
            }

            var genres = await _context.AnimeGenre
                .Where(ag => ag.AnimeId == randomAnime.Id)
                .Join(_context.Genre, ag => ag.GenreId, g => g.Id, (ag, g) => g)
                .ToListAsync();

            var model = new Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>(randomAnime, reviews, genres, userInfos);


            return model;
        }

    }
}