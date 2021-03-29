using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BrimSchedule.Domain.Constants;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrimSchedule.API.Controllers
{
    public class AuthTestController : Controller
    {
        private readonly IHttpContextAccessor _context;

        public AuthTestController(IHttpContextAccessor context)
        {
            _context = context;
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
			var claims = new Dictionary<string, object>()
			{
				{ ClaimsIdentity.DefaultRoleClaimType, RoleNames.Admin }
			};

			return await SetCustomClaims(claims);
		}

		[HttpPost]
		[Authorize(Roles = RoleNames.Admin)]
		public async Task<IActionResult> DemoteAdmin()
		{
			var claims = new Dictionary<string, object>()
			{
				{ ClaimsIdentity.DefaultRoleClaimType, RoleNames.User }
			};

			return await SetCustomClaims(claims);
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

		private async Task<IActionResult> SetCustomClaims(IReadOnlyDictionary<string, object> claims)
		{
			var user = _context.HttpContext?.User;
			if (user == null)
			{
				return BadRequest("HttpContext is null");
			}

			if (user.Identity == null)
			{
				return BadRequest("Users identity is null");
			}

			await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.Identity.Name, claims);

			return Ok();
		}
	}
}
