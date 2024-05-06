using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace animatchWeb.Controllers
{
	public class WatchedController : Controller
	{
		private readonly ApplicationDbContext _context;

		public WatchedController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Watched(int animeId)
		{
			UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

			var likedAnime = new WatchedAnime
			{
				UserId = user.Id,
				AnimeId = animeId,

			};

			_context.WatchedAnime.Add(likedAnime);

			user.WatchedCount++;

			if (user.WatchedCount < 10)
			{
				user.Level = 1;
			}
			else if (user.WatchedCount < 50)
			{
				// Якщо переглядів більше або рівно 10 і менше 50, змінюємо рівень на Intermediate
				user.Level = 2;
			}
			else 
			{
				// Якщо переглядів 50 або більше, змінюємо рівень на Advanced
				user.Level = 3;
			}

			await _context.SaveChangesAsync();

			string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

			// Перенаправляємо користувача на поточну сторінку
			return Redirect(returnUrl);
		}

		[HttpPost]
		public async Task<IActionResult> UnWatched(int animeId)
		{
			UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

			var likedAnime = await _context.WatchedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

			if (likedAnime != null)
			{
				_context.WatchedAnime.Remove(likedAnime);

				user.WatchedCount--;

				if (user.WatchedCount < 10)
				{
					user.Level = 1;
				}
				else if (user.WatchedCount < 50)
				{
					// Якщо переглядів більше або рівно 10 і менше 50, змінюємо рівень на Intermediate
					user.Level = 2;
				}
				else 
				{
					// Якщо переглядів 50 або більше, змінюємо рівень на Advanced
					user.Level = 3;
				}
				await _context.SaveChangesAsync();
			}

			string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

			// Перенаправляємо користувача на поточну сторінку
			return Redirect(returnUrl);
		}

		public async Task<bool> IsWatched(int animeId, string email)
		{
			UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == email);

			var likedAnime = await _context.WatchedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

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
