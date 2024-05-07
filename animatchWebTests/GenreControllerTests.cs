using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace animatchWebTests
{
	public class GenreControllerTests
	{
		[Fact]
		public async Task GetAllGenre_ReturnsListOfGenres()
		{
			// Arrange
			var genres = new List<Genre>
			{
				new Genre { Id = 1, Name = "Action" },
				new Genre { Id = 2, Name = "Adventure" }
			};

			// Create a real instance of DbContextOptions<ApplicationDbContext>
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb1")
				.Options;

			// Seed the in-memory database with test data
			using (var dbContext = new ApplicationDbContext(options))
			{
				dbContext.AddRange(genres);
				dbContext.SaveChanges();
			}

			// Create the controller with the real ApplicationDbContext
			using (var context = new ApplicationDbContext(options))
			{
				var controller = new GenreController(context);

				// Act
				var result = await controller.GetAllGenre();

				// Assert
				Assert.Equal(2, result.Count);
				Assert.Equal("Action", result[0].Name);
				Assert.Equal("Adventure", result[1].Name);
			}
		}

		[Fact]
		public async Task GetGenresForAnime_ReturnsListOfGenresForGivenAnimeId()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb2")
				.Options;

			var animeId = 1;
			var animeGenres = new List<AnimeGenre>
			{
				new AnimeGenre { Id = 1, AnimeId = animeId, GenreId = 1 },
				new AnimeGenre { Id = 2, AnimeId = animeId, GenreId = 2 },
				new AnimeGenre { Id = 3, AnimeId = animeId, GenreId = 3 }
			};

			var genres = new List<Genre>
			{
				new Genre { Id = 1, Name = "Action" },
				new Genre { Id = 2, Name = "Adventure" },
				new Genre { Id = 3, Name = "Comedy" }
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.AddRange(animeGenres);
				context.AddRange(genres);
				context.SaveChanges();
			}

			// Act
			List<string> result;
			using (var context = new ApplicationDbContext(options))
			{
				var controller = new GenreController(context);
				result = await controller.GetGenresForAnime(animeId);
			}

			// Assert
			Assert.Equal(3, result.Count);
			Assert.Contains("Action", result);
			Assert.Contains("Adventure", result);
			Assert.Contains("Comedy", result);
		}
	}
}
