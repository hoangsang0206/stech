using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using System.Security.Claims;
using STech.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Azure.Storage.Blobs;
using STech.Utils;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly string[] ALLOWED_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long MAX_FILE_LENGTH = 5 * 1024 * 1024;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AccountController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
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

        [HttpPost("update"), Authorize]
        public async Task<IActionResult> Update([FromBody] UserUpdateVM update)
        {
            if(ModelState.IsValid)
            {
                if (User.Identity == null)
                {
                    return Unauthorized();
                }

                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return Unauthorized();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                if (await _userService.IsEmailExist(userId, update.Email))
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Email này đã tồn tại"
                    });
                }

                user.FullName = update.FullName;
                user.Email = update.Email;
                user.Phone = update.PhoneNumber;
                user.Gender = update.Gender;
                user.Dob = update.DOB;

                if (await _userService.UpdateUser(user))
                {
                    return Ok(new ApiResponse
                    {
                        Status = true,
                    });
                }
            }

            return BadRequest();
        }

        [HttpPost("upload"), Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest();
            }

            if (!ALLOWED_IMAGE_EXTENSIONS.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Hình ảnh không hợp lệ"
                });
            }

            if (file.Length > MAX_FILE_LENGTH)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = $"Hình ảnh không quá {Convert.ToInt32(MAX_FILE_LENGTH / 1000000)}MB"
                });
            }

            if (User.Identity == null)
            {
                return Unauthorized();
            }

            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return Unauthorized();
            }

            User? user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            string? blobConnectionString = _configuration["Azure:ConnectionString"];
            string? blobContainerName = _configuration["Azure:BlobContainerName"];
            string? blobUrl = _configuration["Azure:BlobUrl"];
            if(blobConnectionString == null || blobContainerName == null || blobUrl == null)
            {
                return BadRequest();
            }

            string fileName = $"{userId}-{RandomUtils.GenerateRandomString(10)}-{Path.GetExtension(file.FileName)}";
            string path = Path.Combine("user-images", fileName);
            
            BlobServiceClient blobServiceClient = new BlobServiceClient(blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);
            BlobClient blobClient = containerClient.GetBlobClient(path);

            using (Stream stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            if(!string.IsNullOrEmpty(user.Avatar))
            {
                BlobClient oldBlobClient = containerClient.GetBlobClient(user.Avatar.Replace($"{blobUrl}{blobContainerName}/", ""));
                await oldBlobClient.DeleteIfExistsAsync();
            }

            user.Avatar = $"{blobUrl}{blobContainerName}/{path}";
            if(await _userService.UpdateUser(user))
            {
                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = user.Avatar
                });
            }

            return BadRequest();
        }
    }
}
