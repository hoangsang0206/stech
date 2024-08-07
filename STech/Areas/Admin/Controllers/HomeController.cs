using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            ViewBag.ActiveSidebar = "home";
            return View();
        }
    }
}
