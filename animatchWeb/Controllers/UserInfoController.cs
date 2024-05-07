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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserInfoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<int> getId(string email)
        {
            int id = await _context.UserInfo.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync();
            return id;
        }   

        public async Task<List<UserInfo>> GetAllUsers()
        {
            var usersList = await _context.UserInfo.ToListAsync();
            return usersList;
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
                    if (originalEmail != userInfo.Email)
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("Index", "Home");
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

        [HttpPost]
        public async Task<IActionResult> ToggleAdmin(int userId, bool isAdmin)
        {
            var user = await _context.UserInfo.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Отримати користувача з Identity за допомогою email
            var userInIdentity = await _userManager.FindByEmailAsync(user.Email);
            if (userInIdentity == null)
            {
                return NotFound();
            }

            // Перевірити наявність ролей
            var roles = await _userManager.GetRolesAsync(userInIdentity);
            bool admin = await _userManager.IsInRoleAsync(userInIdentity, "Admin");
            bool hasAdminRole = roles.Contains("Admin");
            bool hasUserRole = roles.Contains("User");

            // Якщо isAdmin = true, тоді видалимо роль Admin і додамо роль User
            if (admin)
            {
                await _userManager.RemoveFromRoleAsync(userInIdentity, "Admin");
                await _userManager.AddToRoleAsync(userInIdentity, "User");
                user.isAdmin = false; // Оновити isAdmin в базі даних
            }
            // Якщо isAdmin = false, тоді видалимо роль User і додамо роль Admin
            else 
            {
                await _userManager.RemoveFromRoleAsync(userInIdentity, "User");
                await _userManager.AddToRoleAsync(userInIdentity, "Admin");
                user.isAdmin = true; // Оновити isAdmin в базі даних
            }

            // Зберегти зміни в базі даних
            _context.Update(user);
            await _context.SaveChangesAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Email == userInIdentity.Email)
            {
                // Якщо користувач не автентифікований, перенаправляємо його на сторінку входу
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("ContentManagement", "Home"); // Перенаправити на сторінку управління контентом
        }

    }
}
