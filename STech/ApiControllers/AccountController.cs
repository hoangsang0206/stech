using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using System.Security.Claims;
using STech.Services;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM login)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userService.GetUser(login);

                if (user == null || user.UserId == null)
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Sai tên đăng nhập hoặc mật khẩu"
                    });
                }

                if (user.IsActive == false)
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
                    Data = login.ReturnUrl ?? "/"
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                if (await _userService.IsExist(register.RegUserName))
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Tên đăng nhập này đã tồn tại"
                    });
                }

                if (await _userService.IsEmailExist(register.Email))
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Email này đã tồn tại"
                    });
                }

                if (await _userService.CreateUser(register))
                {

                    LoginVM login = new LoginVM
                    {
                        UserName = register.RegUserName,
                        Password = register.RegPassword,
                    };

                    User? user = await _userService.GetUser(login);

                    if (user == null)
                    {
                        return BadRequest();
                    }

                    await SignIn(user);

                    return Ok(new ApiResponse
                    {
                        Status = true,
                        Data = register.ReturnUrl ?? "/"
                    });
                }

                return BadRequest();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
