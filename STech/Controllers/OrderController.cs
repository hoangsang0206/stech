using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Services;
using System.Security.Claims;

namespace STech.Controllers
{
    public class OrderController : Controller
    {
        private readonly AddressService _addressService;
        private readonly IGeocodioService _geocodioService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(AddressService addressService, IGeocodioService geocodioService,
            IUserService userService, IProductService productService, IOrderService orderService, ICartService cartService)
        {
            _addressService = addressService;
            _geocodioService = geocodioService;
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CheckOut(string? pId = null, string? ward = null, string? district = null, string? city = null)
        {
            string? userId = User.FindFirstValue("Id");
            if(userId == null)
            {
                return BadRequest();
            }

            User? user = await _userService.GetUserById(userId);
            if(user == null)
            {
                return BadRequest();
            }

            IEnumerable<UserAddress> userAddresses = await _userService.GetUserAddress(userId);
            user.UserAddresses = userAddresses as List<UserAddress> ?? new List<UserAddress>();
            UserAddress? defaultAddress = userAddresses.FirstOrDefault(t => t.IsDefault == true);

            AddressVM address = new AddressVM();
            city ??= defaultAddress?.ProvinceCode;
            district ??= defaultAddress?.DistrictCode;
            ward ??= defaultAddress?.WardCode;

            address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == city);
            address._District = address._City?.districts.FirstOrDefault(c => c.code == district);
            address._Ward = address._District?.wards.FirstOrDefault(c => c.code == ward);

            Product? product = null; 

            if (pId == null)
            {
                IEnumerable<UserCart> cart = await _cartService.GetUserCart(userId);
                user.UserCarts = cart as List<UserCart> ?? new List<UserCart>();

                if(cart.Count() <= 0)
                {
                    return NotFound();
                }
            }
            else
            {
                product = await _productService.GetProduct(pId);

                if(product == null)
                {
                    return NotFound();
                }
            }

            return View(new Tuple<User, AddressVM, Product?>(user, address, product));
        }

        [Authorize, HttpPost]
        public IActionResult PlaceOrder(OrderVM order)
        {
            if(ModelState.IsValid)
            {

                if(order.pId != null)
                {

                }
                else
                {

                }
            }

            return BadRequest();
        }

        public IActionResult CheckOrder(string oId, string phone)
        {
            return View();
        }
    }
}
