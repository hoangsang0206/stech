using Microsoft.AspNetCore.Mvc;

namespace STech.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{code:int}")]
        public IActionResult Index(int code)
        {
            bool isAdmin = User.IsInRole("admin");
  
            if (code == 404)
            {
                return View("NotFound");
            }

            if (code == 401)
            {
                return View("Unauthorized");
            }

            return View();
        }
    }
}
