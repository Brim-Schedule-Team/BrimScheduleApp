using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrimSchedule.API.Config;
using BrimSchedule.API.Services;
using BrimSchedule.Application;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace BrimSchedule.Tests.API.Services
{
	[TestFixture]
	public class SeederTests
	{
		private const string AdminPhoneNumber = "adminPhoneNumber";

		private Seeder _seeder;
		private Mock<IUserService> _userServiceMock;
		private SeederOptions _seederOptions;

		[SetUp]
		public void Setup()
		{
			_userServiceMock = new Mock<IUserService>();
			_seederOptions = new SeederOptions();
			var options = new Mock<IOptions<SeederOptions>>();
			options.Setup(s => s.Value).Returns(_seederOptions);

			_seeder = new Seeder(_userServiceMock.Object, options.Object);
		}

		[TestCaseSource(nameof(UsersTestCaseSource))]
		public async Task Seed_WhenAdminNotExistsAndPhoneNumberIsConfigured_ShouldAddAdminOrPromoteToAdmin(ICollection<User> users)
		{
			SetupMocks(users, AdminPhoneNumber);

			Func<Task> act = async () => await _seeder.Seed();

			await act.Should().NotThrowAsync();
			users.Should().Contain(s => s.Role == RoleNames.Admin);
		}

		private static readonly List<User>[] UsersTestCaseSource =
		{
			new(),
			new()
			{
				new User()
				{
					Id = "UserId"
				}
			},
			new()
			{
				new User()
				{
					Id = "UserId"
				},
				new User()
				{
					Id = "AdminId",
					PhoneNumber = AdminPhoneNumber
				}
			}
		};

		[Test]
		public async Task Seed_WhenPhoneNumberIsNotConfigured_ShouldNotAddAdminOrPromoteToAdmin()
		{
			var users = new List<User>();

			SetupMocks(users, string.Empty);

			Func<Task> act = async () => await _seeder.Seed();

			await act.Should().NotThrowAsync();
			users.Should().NotContain(s => s.Role == RoleNames.Admin);
		}

		[TestCase(false, true, true)]
		[TestCase(true, false, true)]
		[TestCase(true, true, false)]
		public async Task Seed_WhenServiceResultsIsNotSuccess_ShouldThrowException(
			bool getUsersSuccess,
			bool addUserSuccess,
			bool promoteToAdminSuccess)
		{
			const string errorText = "ServiceResultError";
			var users = new List<User>();

			// empty user list and configured phone number is needed to walk through all service calls
			SetupMocks(
				users,
				AdminPhoneNumber,
				getUsersSuccess,
				addUserSuccess,
				promoteToAdminSuccess,
				errorText);

			Func<Task> act = async () => await _seeder.Seed();

			await act.Should().ThrowAsync<Exception>().WithMessage(errorText);
			users.Should().NotContain(s => s.Role == RoleNames.Admin);
		}

		private void SetupMocks(
			ICollection<User> users,
			string initAdminPhoneNumber,
			bool getUsersSuccess = true,
			bool addUserSuccess = true,
			bool promoteToAdminSuccess = true,
			string errorText = null)
		{
			var adminId = users.FirstOrDefault(s => s.PhoneNumber == initAdminPhoneNumber)?.Id ?? "newAdminId";

			_userServiceMock.Setup(s => s.GetUsers()).ReturnsAsync(new ServiceResult<ICollection<User>>
			{
				Success = getUsersSuccess,
				Content = users,
				ErrorMessage = errorText
			});

			_seederOptions.InitAdminPhone = initAdminPhoneNumber;

			_userServiceMock.Setup(s => s.GetUserByPhone(It.IsAny<string>()))
				.ReturnsAsync(new ServiceResult<User>
				{
					Success = users.Any(s => s.PhoneNumber == initAdminPhoneNumber),
					Content = users.FirstOrDefault(s => s.PhoneNumber == initAdminPhoneNumber)
				});

			_userServiceMock.Setup(s => s.Add(It.IsAny<string>()))
				.ReturnsAsync((string phoneNumber) =>
				{
					User user = null;

					if (addUserSuccess)
					{
						user = new User
						{
							Id = adminId,
							PhoneNumber = phoneNumber
						};

						users.Add(user);
					}

					return new ServiceResult<User>
					{
						Success = addUserSuccess,
						Content = user,
						ErrorMessage = errorText
					};
				});

			_userServiceMock.Setup(s => s.PromoteToAdmin(It.IsAny<string>()))
				.Callback((string id) =>
				{
					if (!promoteToAdminSuccess) return;
					var user = users.FirstOrDefault(s => s.Id == id);
					if (user == null) return;
					user.Role = RoleNames.Admin;
				})
				.ReturnsAsync(new ServiceResult
				{
					Success = promoteToAdminSuccess,
					ErrorMessage = errorText
				});
		}
	}
}
