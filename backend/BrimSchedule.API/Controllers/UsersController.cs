using System.Threading.Tasks;
using BrimSchedule.API.Utility;
using BrimSchedule.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Controllers
{
	[ApiController]
	[Authorize (Roles = "Admin")]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		public UsersController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetUsers()
		{
			var result = await _userService.GetUsers();
			return this.Service(result);
		}

		[HttpPost]
		[Route("{id}/promote")]
		public async Task<IActionResult> PromoteToAdmin(string id)
		{
			var result = await _userService.PromoteToAdmin(id);
			return this.Service(result);
		}

		[HttpPost]
		[Route("{id}/demote")]
		public async Task<IActionResult> DemoteToUser(string id)
		{
			var result = await _userService.DemoteToUser(id);
			return this.Service(result);
		}
	}
}
