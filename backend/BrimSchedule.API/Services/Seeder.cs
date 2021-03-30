using System;
using System.Linq;
using System.Threading.Tasks;
using BrimSchedule.API.Config;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Domain.Constants;
using Microsoft.Extensions.Options;

namespace BrimSchedule.API.Services
{
	public class Seeder
	{
		private readonly IUserService _userService;
		private readonly SeederOptions _options;

		public Seeder(IUserService userService, IOptions<SeederOptions> options)
		{
			_userService = userService;
			_options = options.Value;
		}

		public async Task Seed()
		{
			await SeedAdminUser();
		}

		private async Task SeedAdminUser()
		{
			// try to find users with admin privileges
			var result = await _userService.GetUsers();
			if (!result.Success) throw new Exception(result.ErrorMessage);

			var users = result.Content;

			// if found nothing to do here
			if (users.Any(user => user.Role == RoleNames.Admin)) return;

			// if phoneNumber is not set in config file nothing to do here
			if (string.IsNullOrEmpty(_options.InitAdminPhone)) return;

			// Try to get existent user
			var userResult = await _userService.GetUserByPhone(_options.InitAdminPhone);
			if (!userResult.Success)
			{
				// adding new user with configured phoneNumber
				userResult = await _userService.Add(_options.InitAdminPhone);
				if (!userResult.Success) throw new Exception(userResult.ErrorMessage);
			}

			var userId = userResult.Content.Id;

			// set admin privileges to new created user
			var roleResult = await _userService.PromoteToAdmin(userId);

			if (!roleResult.Success) throw new Exception(roleResult.ErrorMessage);
		}
	}
}
