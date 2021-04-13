using System.Linq;
using System.Threading.Tasks;
using BrimSchedule.API.Utility;
using BrimSchedule.Application.Interfaces.Services;
using BrimSchedule.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Controllers
{
	[Authorize]
	[DevOnly]
    public class AuthTestController : Controller
    {
        private readonly IHttpContextAccessor _context;
		private readonly IUserService _userService;

        public AuthTestController(IHttpContextAccessor context, IUserService userService)
		{
			_context = context;
			_userService = userService;
		}

        // GET
		[AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Login()
		{
			return GetUserContext();
		}

		[HttpGet]
		[Authorize(Roles = RoleNames.Admin)]
		public IActionResult AdminLogin()
		{
			return GetUserContext();
		}

		[HttpPost]
		public async Task<IActionResult> GrantAdminRights()
		{
			var result = await _userService.PromoteToAdmin(GetCurrentUserId());
			return this.Service(result);
		}

		[HttpPost]
		[Authorize(Roles = RoleNames.Admin)]
		public async Task<IActionResult> DemoteAdmin()
		{
			var result = await _userService.DemoteToUser(GetCurrentUserId());
			return this.Service(result);
		}

		private IActionResult GetUserContext()
		{
			var user = _context.HttpContext?.User;
			if (user == null)
			{
				return BadRequest("HttpContext is null");
			}

			return Ok(new
			{
				Name = user.Identity?.Name,
				Claims = user.Claims.Select(s => new { Type = s.Type, Value = s.Value })
			});
		}

		private string GetCurrentUserId() => _context.HttpContext?.User?.Identity?.Name;
	}
}
