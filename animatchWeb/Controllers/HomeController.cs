using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace animatchWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AnimeController _animeController;

        public HomeController(ApplicationDbContext context)
        {
            _animeController = new AnimeController(context);
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var animeList = await _animeController.GetAllAnime(searchString);
            return View(animeList);
        }

        public async Task<IActionResult> Random()
        {
            Random random = new Random();
            var animeList = await _animeController.GetAllAnime("");
            var randomAnime = animeList[random.Next(animeList.Count)];
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
    }
}