using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using Xunit;
using animatchWeb.ViewModels;

namespace animatchWebTests
{
	public class HomeControllerTests
	{
		[Fact]
		public async Task Index_ReturnsViewResult_WithModel()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbHomeController_Index")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				// Додати деякі дані в базу даних
				context.Anime.Add(new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.Genre.Add(new Genre { Id = 1, Name = "Genre 1"});
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				// Створити екземпляр контролера
				var controller = new HomeController(context, null, null);

				// Act
				var result = await controller.Index(null, null);

				// Assert
				var viewResult = Assert.IsType<ViewResult>(result);
				var model = Assert.IsAssignableFrom<Tuple<List<Anime>, List<Genre>>>(viewResult.ViewData.Model);
				Assert.Single(model.Item1); // Перевірка на кількість аніме у моделі
				Assert.Single(model.Item2); // Перевірка на кількість жанрів у моделі
			}
		}

		[Fact]
		public async Task ContentManagement_ReturnsViewResult_WithModel()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDbHomeController_ContentManagement")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				// Додати деякі дані в базу даних
				context.Anime.Add(new Anime { Id = 1, Name = "Anime 1", Photo = "dkpdk", Text = "odkodk" });
				context.UserInfo.Add(new UserInfo {
					Id = 1,
					Email = "test@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof"
				});
				context.Review.Add(new Review { Id = 1, Text = "Review 1", UserId = 1, AnimeId = 1, Rate = 1 });

				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				// Створити екземпляр контролера
				var controller = new HomeController(context, null, null);

				// Act
				var result = await controller.ContentManagement();

				// Assert
				var viewResult = Assert.IsType<ViewResult>(result);
				var model = Assert.IsAssignableFrom<Tuple<List<Anime>, List<ReviewViewModel>, List<UserInfo>>>(viewResult.ViewData.Model);
				Assert.NotNull(model); // Перевірка на наявність моделі
			}
		}
	}
}
