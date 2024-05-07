using animatchWeb.Areas.Identity.Data;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using animatchWeb.ViewModels;

namespace animatchWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnimeController _animeController;
        private readonly LikedAnimeController _likedController;
        private readonly AddedAnimeController _addedAnimeController;
        private readonly ReviewController _reviewController;
        private readonly UserInfoController _userInfoController;
        private readonly GenreController _genreController;
        private readonly DislikedController _dislikedAnimeController;
        private readonly WatchedController _watchedAnimeController;


        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _likedController = new LikedAnimeController(context);
            _animeController = new AnimeController(context);
            _addedAnimeController = new AddedAnimeController(context);
            _reviewController = new ReviewController(context);
            _userInfoController = new UserInfoController(context, userManager, signInManager);
            _genreController = new GenreController(context);
            _watchedAnimeController = new WatchedController(context);
            _dislikedAnimeController = new DislikedController(context);
        }

        public async Task<IActionResult> Index(string searchString, List<int> genreIds)
        {
            var animeList = await _animeController.GetAllAnime(searchString, genreIds);
            var genreList = await _genreController.GetAllGenre();

            var model = new Tuple<List<Anime>, List<Genre>>(animeList, genreList);
            return View(model);
        }

        public async Task<IActionResult> Random()
        {
            //Random random = new Random();
            //var animeList = await _animeController.GetAllAnime("");
            //var randomAnime = animeList[random.Next(animeList.Count)];
            var randomAnime = await _animeController.GetRandomAnime();
            var isLiked = false;
            var isAdded = false;
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.Name;
                isLiked = await _likedController.IsLiked(randomAnime.Item1.Id, userId);
                isAdded = await _addedAnimeController.IsAdded(randomAnime.Item1.Id, userId);
            }
            var model = new Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>, bool, bool>(randomAnime.Item1, randomAnime.Item2, randomAnime.Item3, randomAnime.Item4, isLiked, isAdded);
            return View(model);
            
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

        public async Task<IActionResult> ContentManagement()
        {
            var animeList = await _animeController.GetAllAnime("", new List<int>());
            var reviewList = await _reviewController.GetAllReviews();
            var userList = await _userInfoController.GetAllUsers(); 

            var model = new Tuple<List<Anime>, List<ReviewViewModel>, List<UserInfo>>(animeList, reviewList, userList);
            return View(model);
        }

        public async Task<IActionResult> Recommendation()
        {
            var randomAnime = await _animeController.GetRandomAnime();
            var isLiked = false;
            var isAdded = false;
            var isDisliked = false;
            var isWatched = false;
            
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var userId = await _userInfoController.getId(userName);
                
                randomAnime= await _animeController.getRecommendation(userId);
                
                isLiked = await _likedController.IsLiked(randomAnime.Item1.Id, userName);
                isAdded = await _addedAnimeController.IsAdded(randomAnime.Item1.Id, userName);
                isDisliked = await _dislikedAnimeController.IsDisliked(randomAnime.Item1.Id, userName);
                isWatched = await _watchedAnimeController.IsWatched(randomAnime.Item1.Id, userName);
            }
			var model = new Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>, Tuple<bool, bool, bool, bool>>(
	                    randomAnime.Item1, randomAnime.Item2, randomAnime.Item3, randomAnime.Item4,
	                    Tuple.Create(isLiked, isAdded, isDisliked, isWatched));

			return View(model);
        }
    }
}