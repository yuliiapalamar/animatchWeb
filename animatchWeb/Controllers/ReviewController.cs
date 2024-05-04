using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using animatchWeb.ViewModels;

namespace animatchWeb.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReviewViewModel>> GetAllReviews()
        {
            var reviewList = await _context.Review
                .Join(_context.UserInfo,
                    review => review.UserId,
                    userInfo => userInfo.Id,
                    (review, userInfo) => new { Review = review, UserInfo = userInfo })
                .Join(_context.Anime,
                    reviewUserInfo => reviewUserInfo.Review.AnimeId,
                    anime => anime.Id,
                    (reviewUserInfo, anime) => new ReviewViewModel
                    {
                        Id = reviewUserInfo.Review.Id,
                        UserId = reviewUserInfo.Review.UserId,
                        AnimeId = reviewUserInfo.Review.AnimeId,
                        Username = reviewUserInfo.UserInfo.Username,
                        AnimeName = anime.Name,
                        Text = reviewUserInfo.Review.Text,
                        Rate = reviewUserInfo.Review.Rate
                    })
                .ToListAsync();

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
            return RedirectToAction("Details", "Home", new { id = animeId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            // Знаходимо відгук за його Id
            var review = await _context.Review.FindAsync(reviewId);

            // Перевіряємо, чи відгук існує
            if (review == null)
            {
                return NotFound(); // Якщо відгук не знайдено, повертаємо помилку 404 Not Found
            }

            // Видаляємо відгук з контексту
            _context.Review.Remove(review);

            // Зберігаємо зміни в базі даних
            await _context.SaveChangesAsync();

            // Перенаправляємо на ту ж сторінку (або на іншу, в залежності від вашої логіки)
            return RedirectToAction("ContentManagement", "Home");
        }
    }
}