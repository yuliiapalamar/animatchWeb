using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    }
}