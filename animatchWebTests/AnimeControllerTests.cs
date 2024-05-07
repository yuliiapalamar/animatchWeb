using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace animatchWebTests
{
	public class AnimeControllerTests
	{
		[Fact]
		public async Task GetAllAnime_ReturnsListOfAnime_WhenCalled()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime33")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.Anime.Add(new Anime { Id = 2, Name = "Anime 2", Photo = "dkpdk", Text = "odkodk" });
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.GetAllAnime(null, null);

				// Assert
				Assert.Equal(2, result.Count);
				Assert.Equal("Anime 1", result[0].Name);
				Assert.Equal("Anime 2", result[1].Name);
			}
		}

		[Fact]
		public async Task Details_ReturnsCorrectModel_WhenAnimeExists()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime34")
				.Options;

			var anime = new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" };
			var user = new UserInfo
			{
				Id = 1,
				Email = "test@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok",
				Username = "d,olfof"
			};
			var review = new Review { Id = 1, AnimeId = anime.Id, Text = "dpkk", Rate = 2, UserId = 1 };
			var genre = new Genre { Id = 1, Name = "genre" };
			var animegenre = new AnimeGenre {Id = 1, AnimeId = 1, GenreId = 1 };

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(anime);
				context.UserInfo.Add(user);
				context.Review.Add(review);
				context.Genre.Add(genre);
				context.AnimeGenre.Add(animegenre);
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.Details(anime.Id);

				// Assert
				Assert.NotNull(result);
				Assert.Equal(anime.Id, result.Item1.Id);
				Assert.Equal(review.Id, result.Item2[0].Id);
				Assert.Equal(genre.Id, result.Item3[0].Id);
				Assert.Equal(user.Id, result.Item4[0].Id);
			}
		}

		[Fact]
		public async Task GetRandomAnime_ReturnsRandomAnime()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime35")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.Anime.Add(new Anime { Id = 2, Name = "Anime 2", Photo = "dkpdk", Text = "odkodk" });
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.GetRandomAnime();

				// Assert
				Assert.NotNull(result);
				Assert.IsType<Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>>(result);
			}
		}

		[Fact]
		public async Task Edit_ReturnsNotFound_WhenIdIsNull()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime36")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.Edit(null);

				// Assert
				Assert.IsType<NotFoundResult>(result);
			}
		}

		[Fact]
		public async Task Edit_ReturnsNotFound_WhenAnimeDoesNotExist()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime37")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(new Anime {Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.Edit(999);

				// Assert
				Assert.IsType<NotFoundResult>(result);
			}
		}

		[Fact]
		public async Task Edit_ReturnsViewResult_WhenValidIdIsGiven()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime38")
				.Options;

			var anime = new Anime {Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" };

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(anime);
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.Edit(anime.Id);

				// Assert
				Assert.IsType<ViewResult>(result);
				var viewResult = Assert.IsType<ViewResult>(result);
				var model = Assert.IsAssignableFrom<Anime>(viewResult.ViewData.Model);
				Assert.Equal(anime.Id, model.Id);
			}
		}

		[Fact]
		public async Task Edit_ReturnsRedirectToAction_WhenModelStateIsValid()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime39")
				.Options;

			var anime = new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" };

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(anime);
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.Edit(anime.Id, anime);

				// Assert
				Assert.IsType<RedirectToActionResult>(result);
			}
		}

		[Fact]
		public async Task getRecommendation_ReturnsRandomAnime()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbAnime40")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.Add(new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.Anime.Add(new Anime { Id = 2, Name = "Anime 2", Photo = "dkpdk", Text = "odkodk" });
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new AnimeController(context);

				// Act
				var result = await controller.getRecommendation(1);

				// Assert
				Assert.NotNull(result);
				Assert.IsType<Tuple<Anime, List<Review>, List<Genre>, List<UserInfo>>>(result);
			}
		}

	}
}
