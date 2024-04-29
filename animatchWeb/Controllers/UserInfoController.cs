using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace animatchWeb.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserInfoController(ApplicationDbContext context)
        {
            _context = context;
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

            return View(userInfo);
        }

        public async Task<UserInfo> GetUser(int id)
        {
            var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
    }
}
