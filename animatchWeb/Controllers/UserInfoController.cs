using animatchWeb.Areas.Identity.Data;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace animatchWeb.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserInfoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userInfo = await _context.UserInfo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userInfo == null)
            {
                return NotFound();
            }

            var addedAnimeList = await _context.AddedAnime
                .Where(ag => ag.UserId == id)
                .Join(_context.Anime, ag => ag.AnimeId, g => g.Id, (ag, g) => g)
                .ToListAsync();

            var model = new Tuple<UserInfo, List<Anime>>(userInfo, addedAnimeList);
            return View(model);
        }

        public async Task<UserInfo> GetUser(int id)
        {
            var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
		// GET: UserInfoes/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var userInfo = await _context.UserInfo.FindAsync(id);
			if (userInfo == null)
			{
				return NotFound();
			}
			return View(userInfo);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Email,Name,Level,Text,Photo,WatchedCount,isAdmin")] UserInfo userInfo)
        {
            if (id != userInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Отримати оригінальний email
                    var originalEmail = await _context.UserInfo.Where(u => u.Id == id).Select(u => u.Email).FirstOrDefaultAsync();

                    _context.Update(userInfo);
                    await _context.SaveChangesAsync();

                    // Отримати користувача з Identity за оригінальним email
                    var identityUser = await _userManager.FindByEmailAsync(originalEmail);
                    if (identityUser != null)
                    {
                        identityUser.UserName = userInfo.Email;
                        identityUser.Email = userInfo.Email;

                        // Змінити пароль, якщо він був введений
                        if (!string.IsNullOrWhiteSpace(userInfo.Password))
                        {
                            var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                            await _userManager.ResetPasswordAsync(identityUser, token, userInfo.Password);
                        }

                        // Оновити користувача в Identity
                        await _userManager.UpdateAsync(identityUser);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserInfoExists(userInfo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = userInfo.Id });
            }
            return View(userInfo);
        }

        private bool UserInfoExists(int id)
        {
            return _context.UserInfo.Any(e => e.Id == id);
        }


        public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var userInfo = await _context.UserInfo
				.FirstOrDefaultAsync(m => m.Id == id);
			if (userInfo == null)
			{
				return NotFound();
			}

			return View(userInfo);
		}
	}
}
