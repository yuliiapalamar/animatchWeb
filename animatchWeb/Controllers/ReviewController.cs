using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace animatchWeb.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllReviews()
        {
            var reviewList = await _context.Review.ToListAsync();
            return reviewList;
        }

        public async Task<List<Review>> GetReviewForAnime(int id)
        {
            var reviewList = await _context.Review
                .Where(r => r.AnimeId == id)
                .ToListAsync();
            return reviewList;
        }

        [HttpPost]
        public async Task<IActionResult> LeaveReview(int animeId, string reviewText, int reviewRate)
        {
            // Отримуємо ідентифікатор поточного користувача
            UserInfo user = await _context.UserInfo.FirstOrDefaultAsync(u  => u.Email == User.Identity.Name);
            Debug.WriteLine(user.Id);
            // Створюємо новий відгук
            var review = new Review
            {
                UserId = user.Id,
                AnimeId = animeId,
                Text = reviewText,
                Rate = reviewRate
            };

            // Додаємо відгук до контексту
            _context.Review.Add(review);

            // Зберігаємо зміни в базі даних
            await _context.SaveChangesAsync();

            // Перенаправляємо на сторінку з деталями аніме
            return RedirectToAction("Details", "Anime", new { id = animeId });
        }
    }
}