using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using System.Security.Claims;
using STech.Services;
using Microsoft.AspNetCore.Authorization;
using Azure.Storage.Blobs;
using STech.Utils;
using STech.Services.Services;
using STech.Services.Utils;
using System.Runtime.ConstrainedExecution;

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
        private readonly AddressService _addressService;

        public AccountController(IUserService userService, IConfiguration configuration, 
            AddressService addressService)
        {
            _userService = userService;
            _configuration = configuration;
            _addressService = addressService;
        }

        #region User
        private async Task UserSignIn(User user)
        {
            IEnumerable<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("Id", user.UserId),
                    new Claim("Avatar", user.Avatar ?? "/images/no-image.jpg"),
                    new Claim(ClaimTypes.Role, user.RoleId),
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

                await UserSignIn(user);

                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = user.RoleId == "admin" ? "/admin" : login.ReturnUrl ?? "/"
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

                    await UserSignIn(user);

                    return Ok(new ApiResponse
                    {
                        Status = true,
                        Data = user.RoleId == "admin" ? "/admin" : register.ReturnUrl ?? "/"
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
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return BadRequest();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return BadRequest();
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

        [HttpPut("password"), Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVM password)
        {
            if(ModelState.IsValid)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return BadRequest();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return BadRequest();
                }

                if (user.PasswordHash != password.OldPassword.HashPasswordMD5(user.RandomKey))
                {
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = "Mật khẩu cũ không đúng"
                    });
                }

                user.PasswordHash = password.ConfirmPassword.HashPasswordMD5(user.RandomKey);

                if (await _userService.UpdateUser(user))
                {
                    return Ok(new ApiResponse
                    {
                        Status = true,
                        Message = "Đổi mật khẩu thành công"
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

            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            User? user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return BadRequest();
            }

            string? blobConnectionString = _configuration["Azure:ConnectionString"];
            string? blobContainerName = _configuration["Azure:BlobContainerName"];
            string? blobUrl = _configuration["Azure:BlobUrl"];
            if(blobConnectionString == null || blobContainerName == null || blobUrl == null)
            {
                return BadRequest();
            }

            string fileName = $"{userId}-{RandomUtils.GenerateRandomString(10)}{Path.GetExtension(file.FileName)}";
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

        #endregion



        #region UserAddresses

        [HttpGet("address/{id}"), Authorize]
        public async Task<IActionResult> GetUserAddress(int id)
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            UserAddress? address = await _userService.GetUserAddress(userId, id);
            return Ok(new ApiResponse
            {
                Status = true,
                Data = address
            });
        }

        [HttpGet("address/default"), Authorize]
        public async Task<IActionResult> GetUserDefaultAddress()
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            UserAddress? address = await _userService.GetUserMainAddress(userId);
            return Ok(new ApiResponse
            {
                Status = address != null,
                Data = address
            });
        }

        [HttpGet("address"), Authorize]
        public async Task<IActionResult> GetUserAddresses()
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            IEnumerable<UserAddress> addresses = await _userService.GetUserAddress(userId);
            return Ok(new ApiResponse
            {
                Status = true,
                Data = addresses
            });
        }


        [HttpPost("address"), Authorize]
        public async Task<IActionResult> AddUserAddress([FromBody] AddAddressVM address)
        {
            if(ModelState.IsValid)
            {
                string? userId = User.FindFirstValue("Id");
                if(userId == null)
                {
                    return BadRequest();
                }

                AddressVM.City city = _addressService.Address.Cities.FirstOrDefault(c => c.code == address.CityCode) ?? new AddressVM.City();
                AddressVM.District district = city.districts.FirstOrDefault(c => c.code == address.DistrictCode) ?? new AddressVM.District();
                AddressVM.Ward ward = district.wards.FirstOrDefault(c => c.code == address.WardCode) ?? new AddressVM.Ward();

                if (city.code == null || district.code == null || ward.code == null)
                {
                    return BadRequest();
                }

                UserAddress userAddress = new UserAddress
                {
                    UserId = userId,
                    RecipientName = address.RecipientName,
                    RecipientPhone = address.RecipientPhone,
                    Address = address.Address,
                    Province = city.name_with_type,
                    ProvinceCode = city.code,
                    District = district.name_with_type,
                    DistrictCode = district.code,
                    Ward = ward.name_with_type,
                    WardCode = ward.code,
                    AddressType = address.Type,
                    IsDefault = await _userService.GetUserMainAddress(userId) == null,
                    
                };

                if(await _userService.CreateUserAddress(userAddress))
                {
                    return Ok(new ApiResponse
                    {
                        Status = true,
                    });
                }
            }

            return BadRequest();
        }

        [HttpPut("address/update"), Authorize]
        public async Task<IActionResult> UpdateUserAddress([FromBody] AddAddressVM address)
        {
            if(ModelState.IsValid)
            {
                string? userId = User.FindFirstValue("Id");
                if(userId == null)
                {
                    return BadRequest();
                }

                AddressVM.City city = _addressService.Address.Cities.FirstOrDefault(c => c.code == address.CityCode) ?? new AddressVM.City();
                AddressVM.District district = city.districts.FirstOrDefault(c => c.code == address.DistrictCode) ?? new AddressVM.District();
                AddressVM.Ward ward = district.wards.FirstOrDefault(c => c.code == address.WardCode) ?? new AddressVM.Ward();

                if (city.code == null || district.code == null || ward.code == null)
                {
                    return BadRequest();
                }

                UserAddress? userAddress = await _userService.GetUserAddress(userId, address.Id);
                if(userAddress == null)
                {
                    return BadRequest();
                }

                userAddress.RecipientName = address.RecipientName;
                userAddress.RecipientPhone = address.RecipientPhone;
                userAddress.Address = address.Address;
                userAddress.Province = city.name_with_type;
                userAddress.ProvinceCode = city.code;
                userAddress.District = district.name_with_type;
                userAddress.DistrictCode = district.code;
                userAddress.Ward = ward.name_with_type;
                userAddress.WardCode = ward.code;
                userAddress.AddressType = address.Type;

                if (await _userService.UpdateUserAddress(userAddress))
                {
                    return Ok(new ApiResponse
                    {
                        Status = true,
                    });
                }
            }

            return BadRequest();
        }

        [HttpPut("address/default/{id}"), Authorize]
        public async Task<IActionResult> SetMainAddress(int id)
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            if(await _userService.SetDefaultAddress(userId, id))
            {
                return Ok(new ApiResponse
                {
                    Status = true,
                });
            }

            return BadRequest();
        }

        [HttpDelete("address/{id}"), Authorize]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            if(await _userService.DeleteUserAddress(userId, id))
            {
                return Ok(new ApiResponse
                {
                    Status = true,
                    Message = "Delete successfully"
                });
            }

            return BadRequest();
        }

        #endregion
    }
}
