using Microsoft.AspNetCore.Mvc;
using STech.Filters;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), AdminAuthorize]
    public class ErrorController : Controller
    {
        [Route("/admin/error/{code:int}")]
        public IActionResult Index(int code)
        {
            if (code == 404)
            {
                return View("Error_NotFound");
            }

            if (code == 401)
            {
                return View("Error_Unauthorized");
            }

            return View();
        }

        [Route("/admin/error/unauthorized")]
        public IActionResult Error_Unauthorized()
        {
            return View();
        }

        [Route("/admin/error/notfound")]
        public IActionResult Error_NotFound()
        {
            return View();
        }
    }
}
