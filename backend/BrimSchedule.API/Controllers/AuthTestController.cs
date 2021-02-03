using System.Linq;
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
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Login()
        {
            var user = _context.HttpContext.User;
            var userInfo = new
            {
                Name = user.Identity?.Name,
                Claims = user.Claims.Select(s=> new {Type = s.Type, Value = s.Value})
            };
            return Ok(userInfo);
        }
    }
}