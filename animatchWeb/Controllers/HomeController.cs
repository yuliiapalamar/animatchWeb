using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace animatchWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnimeController _animeController;
        private readonly LikedAnimeController _likedController;
        private readonly AddedAnimeController _addedAnimeController;

        public HomeController(ApplicationDbContext context)
        {
            _likedController = new LikedAnimeController(context);
            _animeController = new AnimeController(context);
            _addedAnimeController = new AddedAnimeController(context);
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var animeList = await _animeController.GetAllAnime(searchString);
            return View(animeList);
        }

        public async Task<IActionResult> Random()
        {
            //Random random = new Random();
            //var animeList = await _animeController.GetAllAnime("");
            //var randomAnime = animeList[random.Next(animeList.Count)];
            var randomAnime = await _animeController.GetRandomAnime();
            return View(randomAnime);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Details(int id)
        {
            var animeDetails = await _animeController.Details(id);
            var isLiked = false;
            var isAdded = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.Name;
                isLiked = await _likedController.IsLiked(id, userId);
                isAdded = await _addedAnimeController.IsAdded(id, userId);
            }
            var model = new Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>, bool, bool>(animeDetails.Item1, animeDetails.Item2, animeDetails.Item3, animeDetails.Item4, isLiked, isAdded);
            return View(model);
        }
    }
}