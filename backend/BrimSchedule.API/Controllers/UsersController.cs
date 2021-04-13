using System.Collections.Generic;
using System.Threading.Tasks;
using BrimSchedule.API.Utility;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Domain.Constants;
using BrimSchedule.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Controllers
{
	[ApiController]
	[Authorize (Roles = RoleNames.Admin)]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Returns all users
		/// </summary>
		/// <returns>List of users</returns>
		[HttpGet]
		[Route("")]
		public async Task<ActionResult<ICollection<User>>> GetUsers()
		{
			var result = await _userService.GetUsers();
			return this.Service(result);
		}

		/// <summary>
		/// Promotes user to admin role
		/// </summary>
		/// <param name="id">User identifier</param>
		/// <returns></returns>
		/// <response code="200"></response>
		/// <response code="400">User not found</response>
		[HttpPost]
		[Route("{id}/promote")]
		public async Task<IActionResult> PromoteToAdmin(string id)
		{
			var result = await _userService.PromoteToAdmin(id);
			return this.Service(result);
		}

		/// <summary>
		/// Demotes admin to user role
		/// </summary>
		/// <param name="id">User identifier</param>
		/// <returns></returns>
		/// <response code="200"></response>
		/// <response code="400">User not found or user demotes himself</response>
		[HttpPost]
		[Route("{id}/demote")]
		public async Task<IActionResult> DemoteToUser(string id)
		{
			var result = await _userService.DemoteToUser(id);
			return this.Service(result);
		}
	}
}
