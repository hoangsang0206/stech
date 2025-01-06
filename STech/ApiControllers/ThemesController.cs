using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThemesController : ControllerBase
    {
        private readonly string[] _themes = { "blue", "purple", "red", "pink", "green" };
        
        [HttpPost]
        public IActionResult Set(string theme)
        {
            if (!Array.Exists(_themes, t => t == theme))
            {
                return BadRequest();
            }

            HttpContext.Response.Cookies.Append("theme", theme, new CookieOptions
            {
                Expires = DateTime.Now.AddYears(10),
                HttpOnly = true
            });

            return Ok();
        }
    }
}
