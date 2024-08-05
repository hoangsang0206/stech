using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Services;

namespace STech.Areas.Admin.ViewComponents
{
    [Area("Admin")]
    public class AdminHeaderViewComponent : ViewComponent
    {
        private readonly IUserService _userService;

        public AdminHeaderViewComponent(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            User? user = await _userService.GetUserById(userId);
            return View(user);
        }
    }
}
