using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace animatchWebTests
{
	public class LikedAnimeControllerTests
	{

		[Fact]
		public async Task LikedList_ReturnsViewWithLikedAnimeList()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb7")
				.Options;

			// Створення тестових даних для аніме
			var animeList = new List<Anime>
			{
				new Anime { Id = 1, Name = "Naruto", Photo = "df,d,f,s", Text = "dmfkdmf" },
				new Anime { Id = 2, Name = "One Piece", Photo = "df,d,f,s", Text = "dmfkdmf" }
			};

			// Створення тестових даних для понравившихся аніме
			var likedAnimeList = new List<LikedAnime>
			{
				new LikedAnime { UserId = 1, AnimeId = 1 },
				new LikedAnime { UserId = 1, AnimeId = 2 }
			};

			// Створення контексту бази даних та додавання тестових даних
			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.AddRange(animeList);
				context.LikedAnime.AddRange(likedAnimeList);
				await context.SaveChangesAsync();
			}

			// Створення контролера
			var controller = new LikedAnimeController(new ApplicationDbContext(options));

			// Act
			var result = await controller.LikedList(1) as ViewResult;
			var model = result.ViewData.Model as List<Anime>;

			// Assert
			Assert.NotNull(result);
			Assert.NotNull(model);
			Assert.Equal(2, model.Count);
		}

		[Fact]
		public async Task Like_ReturnsRedirectToReferer()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb8")
				.Options;

			// Створення тестових даних користувача
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

			// Створення контексту бази даних і додання тестових даних користувача
			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(user);
				await context.SaveChangesAsync();
			}

			// Створення контексту HTTP з визначеним заголовком "Referer"
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["Referer"] = "http://example.com/previous-page";
			httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
		new Claim(ClaimTypes.Name, "test@example.com"),
			}));

			// Створення контролера з використанням контексту бази даних та контексту HTTP
			var controller = new LikedAnimeController(new ApplicationDbContext(options))
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = httpContext
				}
			};

			var animeId = 1;

			// Act
			var result = await controller.Like(animeId) as RedirectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("http://example.com/previous-page", result.Url);

			// Перевірка, чи був створений новий запис AddedAnime в базі даних
			using (var context = new ApplicationDbContext(options))
			{
				Assert.Single(context.LikedAnime);
				Assert.Equal(animeId, context.LikedAnime.First().AnimeId);
				Assert.Equal(user.Id, context.LikedAnime.First().UserId);
			}
		}

		[Fact]
		public async Task Unlike_ReturnsRedirectToReferer()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb9")
				.Options;

			// Створення тестового користувача
			var user = new UserInfo
			{
				Id = 1,
				Email = "test@example.com",
				Name = "Test User",
				Password = "password",
				Photo = "photo.jpg",
				Text = "Some text",
				Username = "testuser"
			};

			// Додавання тестового користувача до бази даних
			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(user);
				await context.SaveChangesAsync();
			}

			// Створення контексту HTTP з визначеним заголовком "Referer" та користувачем
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Headers["Referer"] = "http://example.com/previous-page";
			httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
		new Claim(ClaimTypes.Name, "test@example.com"),
			}));

			// Створення контролера з використанням контексту бази даних та контексту HTTP
			var controller = new LikedAnimeController(new ApplicationDbContext(options))
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = httpContext,
				}
			};

			var animeId = 1;

			// Додавання запису у список вибраних аніме до бази даних
			using (var context = new ApplicationDbContext(options))
			{
				context.LikedAnime.Add(new LikedAnime { UserId = user.Id, AnimeId = animeId });
				await context.SaveChangesAsync();
			}

			// Act
			var result = await controller.Unlike(animeId) as RedirectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("http://example.com/previous-page", result.Url);

			// Перевірка, чи запис був видалений з бази даних
			using (var context = new ApplicationDbContext(options))
			{
				Assert.Equal(0, context.LikedAnime.Count());
				Assert.False(context.LikedAnime.Any(a => a.AnimeId == animeId));
			}
		}

		[Fact]
		public async Task IsLiked_ReturnsTrueIfAnimeIsAdded()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb10")
				.Options;

			var animeId = 1;
			var userEmail = "test@example.com";
			using (var context = new ApplicationDbContext(options))
			{
				context.LikedAnime.Add(new LikedAnime { UserId = 1, AnimeId = animeId });
				context.UserInfo.Add(new UserInfo
				{
					Email = userEmail,
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof"
				});
				context.SaveChanges();
			}

			var controller = new LikedAnimeController(new ApplicationDbContext(options));

			// Act
			var result = await controller.IsLiked(animeId, userEmail);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsLiked_ReturnsFalseIfAnimeIsNotAdded()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb11")
				.Options;

			var animeId = 1;
			var userEmail = "test@example.com";
			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(new UserInfo
				{
					Email = userEmail,
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof"
				});
				context.SaveChanges();
			}

			var controller = new LikedAnimeController(new ApplicationDbContext(options));

			// Act
			var result = await controller.IsLiked(animeId, userEmail);

			// Assert
			Assert.False(result);
		}
	}
}
