using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace animatchWebTests
{
	public class DislikedControllerTests
	{
		[Fact]
		public async Task Dislike_ReturnsRedirectToReferer()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb16")
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
			var controller = new DislikedController(new ApplicationDbContext(options))
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = httpContext
				}
			};

			var animeId = 1;

			// Act
			var result = await controller.Dislike(animeId) as RedirectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("http://example.com/previous-page", result.Url);

			// Перевірка, чи був створений новий запис AddedAnime в базі даних
			using (var context = new ApplicationDbContext(options))
			{
				Assert.Single(context.DislikedAnime);
				Assert.Equal(animeId, context.DislikedAnime.First().AnimeId);
				Assert.Equal(user.Id, context.DislikedAnime.First().UserId);
			}
		}

		[Fact]
		public async Task UnDisliked_ReturnsRedirectToReferer()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb17")
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
			var controller = new DislikedController(new ApplicationDbContext(options))
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
				context.DislikedAnime.Add(new DislikedAnime { UserId = user.Id, AnimeId = animeId });
				await context.SaveChangesAsync();
			}

			// Act
			var result = await controller.UnDisliked(animeId) as RedirectResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("http://example.com/previous-page", result.Url);

			// Перевірка, чи запис був видалений з бази даних
			using (var context = new ApplicationDbContext(options))
			{
				Assert.Equal(0, context.DislikedAnime.Count());
				Assert.False(context.DislikedAnime.Any(a => a.AnimeId == animeId));
			}
		}

		[Fact]
		public async Task IsDisliked_ReturnsTrueIfAnimeIsAdded()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb18")
				.Options;

			var animeId = 1;
			var userEmail = "test@example.com";
			using (var context = new ApplicationDbContext(options))
			{
				context.DislikedAnime.Add(new DislikedAnime { UserId = 1, AnimeId = animeId });
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

			var controller = new DislikedController(new ApplicationDbContext(options));

			// Act
			var result = await controller.IsDisliked(animeId, userEmail);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task IsDisliked_ReturnsFalseIfAnimeIsNotAdded()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb19")
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

			var controller = new DislikedController(new ApplicationDbContext(options));

			// Act
			var result = await controller.IsDisliked(animeId, userEmail);

			// Assert
			Assert.False(result);
		}
	}
}
