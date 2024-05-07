using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using animatchWeb.ViewModels;
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
	public class ReviewControllerTests
	{
		[Fact]
		public async Task GetAllReviews_ReturnsListOfReviewViewModels()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb20")
				.Options;

			var user1 = new UserInfo {
				Id = 1,
				Email = "test@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok",
				Username = "d,olfof"
			};
			var user2 = new UserInfo {
				Id = 2,
				Email = "test2@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok",
				Username = "d,olfofdsfsf"
			};

			var anime1 = new Anime { Id = 1, Name = "Anime1", Text = "ldspflspfl", Photo = "slfpldfdl" };
			var anime2 = new Anime { Id = 2, Name = "Anime2", Text = "ldspflspfl", Photo = "slfpldfdl" };

			var reviews = new List<Review>
			{
				new Review { Id = 1, UserId = 1, AnimeId = 1, Text = "Review 1", Rate = 5 },
				new Review { Id = 2, UserId = 2, AnimeId = 1, Text = "Review 2", Rate = 4 },
				new Review { Id = 3, UserId = 1, AnimeId = 2, Text = "Review 3", Rate = 3 }
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.AddRange(user1, user2);
				context.Anime.AddRange(anime1, anime2);
				context.Review.AddRange(reviews);
				await context.SaveChangesAsync();
			}

			var controller = new ReviewController(new ApplicationDbContext(options));

			// Act
			var result = await controller.GetAllReviews();

			// Assert
			Assert.NotNull(result);
			Assert.IsType<List<ReviewViewModel>>(result);
			Assert.Equal(3, result.Count);
		}

		[Fact]
		public async Task GetReviewForAnime_ReturnsListOfReviewsForSpecifiedAnimeId()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb21")
				.Options;

			var anime1 = new Anime {Id = 1, Name = "Anime1", Text = "ldspflspfl", Photo = "slfpldfdl" };
			var anime2 = new Anime {Id = 2, Name = "Anime2", Text = "ldspflspfl", Photo = "slfpldfdl" };

			var reviews = new List<Review>
			{
				new Review { Id = 1, AnimeId = 1, Text = "Review 1", Rate = 5 },
				new Review { Id = 2, AnimeId = 1, Text = "Review 2", Rate = 4 },
				new Review { Id = 3, AnimeId = 2, Text = "Review 3", Rate = 3 }
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.Anime.AddRange(anime1, anime2);
				context.Review.AddRange(reviews);
				await context.SaveChangesAsync();
			}

			var controller = new ReviewController(new ApplicationDbContext(options));

			// Act
			var result = await controller.GetReviewForAnime(1);

			// Assert
			Assert.NotNull(result);
			Assert.IsType<List<Review>>(result);
			Assert.Equal(2, result.Count);
		}

		[Fact]
		public async Task LeaveReview_RedirectsToDetailsActionOfHomeController()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb22")
				.Options;

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
			var animeId = 1;
			var reviewText = "Test review";
			var reviewRate = 5;

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(user);
				context.Anime.Add(new Anime { Id = animeId, Name = "dlfpdlf", Photo = "dfdpfldfl", Text = "dpffpfdpkf" });
				await context.SaveChangesAsync();
			}

			var controller = new ReviewController(new ApplicationDbContext(options));

			// Create a mock HttpContext
			var httpContext = new DefaultHttpContext();
			httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name, "test@example.com"), // Set the user email
            }));

			// Assign the mock HttpContext to the controller's HttpContext
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext
			};

			// Act
			var result = await controller.LeaveReview(animeId, reviewText, reviewRate) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Details", result.ActionName);
			Assert.Equal("Home", result.ControllerName);
			Assert.Equal(animeId, result.RouteValues["id"]);
		}

		[Fact]
		public async Task DeleteReview_RemovesReviewFromDatabaseAndRedirectsToContentManagementActionOfHomeController()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb23")
				.Options;

			var reviewId = 1;
			var review = new Review { Id = reviewId, AnimeId = 1, Text = "flpdldpfldlf" };

			using (var context = new ApplicationDbContext(options))
			{
				context.Review.Add(review);
				await context.SaveChangesAsync();
			}

			var controller = new ReviewController(new ApplicationDbContext(options));

			// Act
			var result = await controller.DeleteReview(reviewId) as RedirectToActionResult;

			// Assert
			using (var context = new ApplicationDbContext(options))
			{
				Assert.Null(context.Review.Find(reviewId));
			}
			Assert.NotNull(result);
			Assert.Equal("ContentManagement", result.ActionName);
			Assert.Equal("Home", result.ControllerName);
		}
	}
}
