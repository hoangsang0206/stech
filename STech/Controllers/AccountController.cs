using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using System.Security.Claims;

namespace STech.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue("Id");

            if (userId == null)
            {
                return BadRequest();
            }

            User user = await _userService.GetUserById(userId) ?? new User();
            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tài khoản", "")
            };

            return View(new Tuple<User, IEnumerable<Breadcrumb>>(user, breadcrumbs));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
