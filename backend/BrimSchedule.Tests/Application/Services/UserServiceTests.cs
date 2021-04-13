using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using BrimSchedule.Application.Interfaces.Repositories;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Application.Services;
using BrimSchedule.Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace BrimSchedule.Tests.Application.Services
{
	[TestFixture]
	public class UserServiceTests
	{
		private IUserService _userService;
		private HttpContext _httpContext;
		private Mock<IUserRepository> _userRepositoryMock;
		private Mock<IUnitOfWork> _unitOfWorkMock;

		[SetUp]
		public void Setup()
		{
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_userRepositoryMock = new Mock<IUserRepository>();
			var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

			_httpContext = new DefaultHttpContext();
			httpContextAccessorMock.Setup(s => s.HttpContext).Returns(_httpContext);

			_userService = new UserService(
				_unitOfWorkMock.Object,
				_userRepositoryMock.Object,
				httpContextAccessorMock.Object
			);


		}

		[TearDown]
		public void TearDown()
		{
			_userRepositoryMock.VerifyAll();
			_userRepositoryMock.VerifyNoOtherCalls();
		}

		[Test]
		public async Task GetUsers_ShouldReturnAllUsers_WithProfiles()
		{
			const string userWithProfileId = "userWithProfileId";
			const string userWithoutProfileId = "userWithoutProfileId";

			ICollection<User> users = new List<User>
			{
				new()
				{
					Id = userWithProfileId
				},
				new ()
				{
					Id = userWithoutProfileId
				}
			};

			ICollection<Profile> profiles = new List<Profile>
			{
				new()
				{
					UserId = userWithProfileId
				}
			};

			_userRepositoryMock
				.Setup(s => s.ListAllUsers(It.IsAny<CancellationToken>()))
				.ReturnsAsync(users);

			SetupProfileRepositoryGet(profiles);

			var result = await _userService.GetUsers();

			result.Success.Should().BeTrue();
			result.Content.Should().HaveSameCount(users);
			result.Content.Should().OnlyContain(user => user.Profile != null);
		}

		[Test]
		public async Task GetUserById_WhenUserExists_ShouldReturnUser_WithProfile()
		{
			const string userId = "userId";

			var user = new User()
			{
				Id = userId
			};

			SetupUserRepositoryGetById(user);
			SetupProfileRepositoryGet(new List<Profile>());

			var result = await _userService.GetUserById(userId);

			result.Success.Should().BeTrue();
			result.Content.Id.Should().BeEquivalentTo(userId);
			result.Content.Profile.Should().NotBeNull();
		}

		[Test]
		public async Task GetUserById_WhenUserNotExists_ShouldReturnFailResult()
		{
			const string userId = "userId";

			SetupUserRepositoryGetById(null);

			var result = await _userService.GetUserById(userId);

			result.Success.Should().BeFalse();
			result.ErrorMessage.Should().BeEquivalentTo($"User id '{userId}' not found");
		}

		[Test]
		public async Task GetUserByPhone_WhenUserExists_ShouldReturnUser_WithProfile()
		{
			const string userPhoneNumber = "userPhoneNumber";

			var user = new User()
			{
				PhoneNumber = userPhoneNumber
			};

			SetupUserRepositoryGetByPhoneNumber(user);
			SetupProfileRepositoryGet(new List<Profile>());

			var result = await _userService.GetUserByPhone(userPhoneNumber);

			result.Success.Should().BeTrue();
			result.Content.PhoneNumber.Should().BeEquivalentTo(userPhoneNumber);
			result.Content.Profile.Should().NotBeNull();
		}

		[Test]
		public async Task GetUserByPhone_WhenUserNotExists_ShouldReturnFailResult()
		{
			const string userPhoneNumber = "userPhoneNumber";

			SetupUserRepositoryGetByPhoneNumber(null);

			var result = await _userService.GetUserByPhone(userPhoneNumber);

			result.Success.Should().BeFalse();
			result.ErrorMessage.Should().BeEquivalentTo($"User with phoneNumber '{userPhoneNumber}' not found");
		}

		[Test]
		public async Task Add_WhenUserNotExists_ShouldAddUser_AndReturnNewUser()
		{
			const string userPhoneNumber = "userPhoneNumber";

			var user = new User
			{
				PhoneNumber = userPhoneNumber
			};

			SetupUserRepositoryGetByPhoneNumber(null);

			_userRepositoryMock.Setup(s => s.Insert(It.IsAny<User>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			var result = await _userService.Add(userPhoneNumber);

			result.Success.Should().BeTrue();
			result.Content.PhoneNumber.Should().BeEquivalentTo(userPhoneNumber);
		}

		[Test]
		public async Task Add_WhenUserExists_ShouldReturnFailResult()
		{
			const string userPhoneNumber = "userPhoneNumber";

			var user = new User
			{
				PhoneNumber = userPhoneNumber
			};

			SetupUserRepositoryGetByPhoneNumber(user);

			var result = await _userService.Add(userPhoneNumber);

			result.Success.Should().BeFalse();
			result.ErrorMessage.Should().BeEquivalentTo($"User with phoneNumber '{userPhoneNumber} already exists");
		}

		[Test]
		public async Task PromoteToAdmin_WhenUserExists_ShouldPromote()
		{
			const string userId = "userId";

			var user = new User { Id = userId };

			SetupUserRepositoryGetById(user);

			_userRepositoryMock.Setup(s =>
				s.SetRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

			var result = await _userService.PromoteToAdmin(userId);

			result.Success.Should().BeTrue();
		}

		[Test]
		public async Task PromoteToAdmin_WhenUserNotExists_ShouldReturnFailResult()
		{
			const string userId = "userId";

			SetupUserRepositoryGetById(null);

			var result = await _userService.PromoteToAdmin(userId);

			result.Success.Should().BeFalse();
			result.ErrorMessage.Should().BeEquivalentTo($"User id '{userId}' not found");
		}

		[Test]
		public async Task DemoteToUser_WhenUserExists_ShouldDemote()
		{
			const string userId = "userId";

			var user = new User { Id = userId };

			SetupUserRepositoryGetById(user);
			_userRepositoryMock.Setup(s =>
				s.SetRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));

			var result = await _userService.DemoteToUser(userId);

			result.Success.Should().BeTrue();
		}

		[Test]
		public async Task DemoteToUser_WhenUserNotExists_ShouldReturnFailResult()
		{
			const string userId = "userId";

			SetupUserRepositoryGetById(null);

			var result = await _userService.DemoteToUser(userId);

			result.Success.Should().BeFalse();
			result.ErrorMessage.Should().BeEquivalentTo($"User id '{userId}' not found");
		}

		[Test]
		public async Task DemoteToUser_WhenIsSameUser_ShouldReturnFailResult()
		{
			const string userId = "userId";

			_httpContext.User = new GenericPrincipal(new GenericIdentity(userId), Array.Empty<string>());

			var result = await _userService.DemoteToUser(userId);

			result.Success.Should().BeFalse();
			result.ErrorMessage.Should().BeEquivalentTo("User can't demote himself");
		}

		[Test]
		public async Task Delete_ShouldDelete()
		{
			const string userId = "userId";

			_userRepositoryMock.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<CancellationToken>()));

			var result = await _userService.Delete(userId);

			result.Success.Should().BeTrue();
		}

		private void SetupProfileRepositoryGet(IEnumerable<Profile> profiles)
		{
			var profilesRepositoryMock = new Mock<IRepository<Profile>>();
			profilesRepositoryMock.Setup(s => s.Get(p => p.UserId == It.IsAny<string>(), null)).Returns(profiles);

			_unitOfWorkMock.Setup(s => s.Profiles).Returns(profilesRepositoryMock.Object);
		}

		private void SetupUserRepositoryGetById(User user)
		{
			_userRepositoryMock
				.Setup(s => s.GetById(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);
		}

		private void SetupUserRepositoryGetByPhoneNumber(User user)
		{
			_userRepositoryMock
				.Setup(s => s.GetByPhoneNumber(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);
		}
	}
}
