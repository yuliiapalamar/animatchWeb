using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace animatchWeb.Controllers
{
	public class DislikedController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DislikedController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> Dislike(int animeId)
		{
			UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

			var likedAnime = new DislikedAnime
			{
				UserId = user.Id,
				AnimeId = animeId,

			};

			_context.DislikedAnime.Add(likedAnime);

			await _context.SaveChangesAsync();

			string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

			// Перенаправляємо користувача на поточну сторінку
			return Redirect(returnUrl);
		}

		[HttpPost]
		public async Task<IActionResult> UnDisliked(int animeId)
		{
			UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

			var likedAnime = await _context.DislikedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

			if (likedAnime != null)
			{
				_context.DislikedAnime.Remove(likedAnime);
				await _context.SaveChangesAsync();
			}

			string returnUrl = HttpContext.Request.Headers["Referer"].ToString();

			// Перенаправляємо користувача на поточну сторінку
			return Redirect(returnUrl);
		}

		public async Task<bool> IsDisliked(int animeId, string email)
		{
			UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email == email);
			var likedAnime = await _context.DislikedAnime.FirstOrDefaultAsync(l => l.AnimeId == animeId && l.UserId == user.Id);

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
