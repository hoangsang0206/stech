using Microsoft.AspNetCore.Mvc;

namespace STech.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
