using animatchWeb.Areas.Identity.Data;
using animatchWeb.Controllers;
using animatchWeb.Data;
using animatchWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace animatchWebTests
{
	public class UserInfoControllerTests
	{
		[Fact]
		public async Task GetAllUsers_ReturnsListOfUsers()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb24")
				.Options;

			var users = new List<UserInfo>
			{
				new UserInfo { Id = 1, 
					Email = "test@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof" },
				new UserInfo { Id = 2,
					Email = "test2@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof" },
				new UserInfo { Id = 3,
					Email = "test3@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof"}
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.AddRange(users);
				await context.SaveChangesAsync();
			}

			var controller = new UserInfoController(new ApplicationDbContext(options), null, null);

			// Act
			var result = await controller.GetAllUsers();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(users.Count, result.Count);
			Assert.Equal(users.Select(u => u.Username), result.Select(u => u.Username));
		}

		[Fact]
		public async Task Details_ReturnsViewWithUserInfoAndAddedAnime()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb25")
				.Options;

			var userInfo = new UserInfo
			{
				Id = 1,
				Username = "testuser",
				Email = "test@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok",
			};

			var addedAnime = new List<Anime>
			{
				new Anime { Id = 1, Name = "Anime1", Photo = "dplfpdlf", Text = "spldcfpdlf" },
				new Anime { Id = 2, Name = "Anime2", Photo = "dplfpdlf", Text = "spldcfpdlf"  },
				new Anime { Id = 3, Name = "Anime3", Photo = "dplfpdlf", Text = "spldcfpdlf"  }
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(userInfo);
				context.Anime.AddRange(addedAnime);
				context.AddedAnime.AddRange(
					new AddedAnime { UserId = userInfo.Id, AnimeId = 1 },
					new AddedAnime { UserId = userInfo.Id, AnimeId = 2 },
					new AddedAnime { UserId = userInfo.Id, AnimeId = 3}
				);
				await context.SaveChangesAsync();
			}

			var controller = new UserInfoController(new ApplicationDbContext(options), null, null);

			// Act
			var result = await controller.Details(userInfo.Id) as ViewResult;
			var model = result.Model as Tuple<UserInfo, List<Anime>>;

			// Assert
			Assert.NotNull(result);
			Assert.NotNull(model);
			Assert.Equal(userInfo.Id, model.Item1.Id);
			Assert.Equal(userInfo.Username, model.Item1.Username);
			Assert.Equal(addedAnime.Count, model.Item2.Count);
		}

		[Fact]
		public async Task GetUser_ReturnsUserInfo()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb26")
				.Options;

			var users = new List<UserInfo>
			{
				new UserInfo { Id = 1, 
					Email = "test@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof" },
				new UserInfo { Id = 2,
					Email = "test2@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof" },
				new UserInfo { Id = 3,
					Email = "test3@example.com",
					Name = "dpldpp",
					Password = "pldpldpl",
					Photo = "plfpf",
					Text = "dfkfok",
					Username = "d,olfof"}
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.AddRange(users);
				await context.SaveChangesAsync();
			}

			var controller = new UserInfoController(new ApplicationDbContext(options), null, null);

			// Act
			var result = await controller.GetUser(1);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(1, result.Id);
			Assert.Equal(users[0].Username, result.Username);
		}

		[Fact]
		public async Task Edit_Get_ReturnsViewWithUserInfo()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb27")
				.Options;

			var userInfo = new UserInfo { Id = 1,
				Email = "test@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok",
				Username = "d,olfof"
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(userInfo);
				await context.SaveChangesAsync();
			}

			var controller = new UserInfoController(new ApplicationDbContext(options), null, null);

			// Act
			var result = await controller.Edit(userInfo.Id) as ViewResult;
			var model = result.Model as UserInfo;

			// Assert
			Assert.NotNull(result);
			Assert.NotNull(model);
			Assert.Equal(userInfo.Id, model.Id);
			Assert.Equal(userInfo.Username, model.Username);
		}

		[Fact]
		public async Task Edit_Post_RedirectsToDetailsActionOfHomeController()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb28")
				.Options;

			var userInfo = new UserInfo
			{
				Id = 1,
				Email = "test@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok",
				Username = "d,olfof"
			};

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(userInfo);
				await context.SaveChangesAsync();
			}

			var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
			var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
			userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
							.ReturnsAsync(new ApplicationUser { UserName = "test@example.com", Email = "test@example.com" });

			var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(userManagerMock.Object,
																			 Mock.Of<IHttpContextAccessor>(),
																			 Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
																			 null, null, null, null);

			var controller = new UserInfoController(new ApplicationDbContext(options), userManagerMock.Object, signInManagerMock.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
					{
				new Claim(ClaimTypes.Name, "test@example.com")
					}))
				}
			};

			var editedUserInfo = new UserInfo
			{
				Id = userInfo.Id,
				Username = "updatedUser",
				Email = "updated@example.com",
				Name = "dpldpp",
				Password = "pldpldpl",
				Photo = "plfpf",
				Text = "dfkfok"
			};

			// Act
			var result = await controller.Edit(editedUserInfo.Id, editedUserInfo) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("Home", result.ControllerName);

			using (var context = new ApplicationDbContext(options))
			{
				var savedUserInfo = await context.UserInfo.FindAsync(editedUserInfo.Id);
				Assert.NotNull(savedUserInfo);
				Assert.Equal(editedUserInfo.Id, savedUserInfo.Id);
				Assert.Equal(editedUserInfo.Username, savedUserInfo.Username);
				Assert.Equal(editedUserInfo.Email, savedUserInfo.Email);
				Assert.Equal(editedUserInfo.Name, savedUserInfo.Name);
				Assert.Equal(editedUserInfo.Password, savedUserInfo.Password);
				Assert.Equal(editedUserInfo.Photo, savedUserInfo.Photo);
				Assert.Equal(editedUserInfo.Text, savedUserInfo.Text);
			}
			
		}

		[Fact]
		public async Task ToggleAdmin_TogglesAdminRole()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb29")
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

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(user);
				await context.SaveChangesAsync();
			}

			var userStore = new Mock<IUserStore<ApplicationUser>>();
			var userManager = new Mock<UserManager<ApplicationUser>>(
				userStore.Object, null, null, null, null, null, null, null, null);

			var signInManager = new Mock<SignInManager<ApplicationUser>>(
				userManager.Object,
				Mock.Of<IHttpContextAccessor>(),
				Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
				null, null, null, null);

			var controller = new UserInfoController(new ApplicationDbContext(options), userManager.Object, signInManager.Object);

			// Act
			var result = await controller.ToggleAdmin(user.Id, true) as RedirectToActionResult;

			using (var context = new ApplicationDbContext(options))
			{
				var updatedUser = await context.UserInfo.FindAsync(user.Id);
				Assert.False(updatedUser.isAdmin);
			}
		}

		[Fact]
		public async Task Delete_ReturnsNotFound_WhenIdIsNull()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb30")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new UserInfoController(context, null, null);

				// Act
				var result = await controller.Delete(null);

				// Assert
				Assert.IsType<NotFoundResult>(result);
			}
		}

		[Fact]
		public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb31")
				.Options;

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new UserInfoController(context, null, null);

				// Act
				var result = await controller.Delete(1);

				// Assert
				Assert.IsType<NotFoundResult>(result);
			}
		}

		[Fact]
		public async Task Delete_ReturnsViewResult_WhenUserExists()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDb32")
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

			using (var context = new ApplicationDbContext(options))
			{
				context.UserInfo.Add(user);
				context.SaveChanges();
			}

			using (var context = new ApplicationDbContext(options))
			{
				var controller = new UserInfoController(context, null, null);

				// Act
				var result = await controller.Delete(user.Id);

				// Assert
				Assert.IsType<ViewResult>(result);
				var viewResult = Assert.IsType<ViewResult>(result);
				var model = Assert.IsAssignableFrom<UserInfo>(viewResult.ViewData.Model);
				Assert.Equal(user.Id, model.Id);
			}
		}

	}
}
