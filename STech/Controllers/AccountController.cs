using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        public IActionResult Index()
        {
            return View();
        }

        private async Task SignIn(User user)
        {
            IEnumerable<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("Id", user.UserId)
                };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login, string? returnUrl)
        {
            if(ModelState.IsValid)
            {
                User user = await _userService.GetUser(login);

                if (user == null || user.UserId == null)
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Sai tên đăng nhập hoặc mật khẩu"
                    });
                }

                if(user.IsActive == false)
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Tài khoản đã bị khóa"
                    });
                }

                await SignIn(user);

                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = returnUrl ?? "/"
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                if(await _userService.IsExist(register.RegUserName)) {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Tên đăng nhập này đã tồn tại"
                    });
                }

                if(await _userService.IsEmailExist(register.Email)) {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Email này đã tồn tại"
                    });
                }

                if(await _userService.CreateUser(register))
                {

                    LoginVM login = new LoginVM
                    {
                        UserName = register.RegUserName,
                        Password = register.RegPassword,
                    };

                    User user = await _userService.GetUser(login);
                    await SignIn(user);

                    return Ok(new ApiResponse
                    {
                        Status = true,
                        Data = returnUrl ?? "/"
                    });
                }

                return BadRequest();
            }
            else
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
