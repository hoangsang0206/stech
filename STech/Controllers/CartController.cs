using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace STech.Controllers
{
    public class CartController : Controller
    {
        private readonly string CART_KEY = "CART";
        private readonly int COOKIE_EXPIRE = 60;

        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public CartController(ICartService cartService, IProductService productService, IUserService userService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
        }


        private void SaveCartToCookie(IEnumerable<CartVM> cart)
        {
            if (cart.Count() <= 0)
            {
                return;
            }

            string cartJson = JsonConvert.SerializeObject(cart);
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(cartJson);
            string base64String = Convert.ToBase64String(bytesToEncode);

            Response.Cookies.Append(CART_KEY, base64String, new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddDays(COOKIE_EXPIRE)
            });
        }

        private List<CartVM> GetCartFromCookie()
        {
            if (Request.Cookies.TryGetValue(CART_KEY, out var base64String))
            {
                if (!string.IsNullOrEmpty(base64String))
                {
                    byte[] bytesToDecode = Convert.FromBase64String(base64String);
                    string cartJson = Encoding.UTF8.GetString(bytesToDecode);
                    return JsonConvert.DeserializeObject<List<CartVM>>(cartJson) ?? new List<CartVM>();
                }
            }

            return new List<CartVM>();
        }

        private void DeleteCookieCart()
        {
            Response.Cookies.Delete(CART_KEY);
        }

        private int UpdateCartItemQuantity(string type, int oldQty, int newQty)
        {
            switch (type)
            {
                case "plus":
                    oldQty += 1;
                    break;
                case "minus":
                    oldQty -= 1;
                    break;
                default:
                    oldQty = newQty;
                    break;
            }

            if(oldQty <= 0)
            {
                oldQty = 1;
            }

            return oldQty;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<UserCart> cart = new List<UserCart>();
            List<CartVM> cartFromCookie = GetCartFromCookie();

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return BadRequest();
                }

                if(cartFromCookie.Count() > 0) { 
                    foreach(CartVM c in cartFromCookie)
                    {
                        if (c.ProductId == null)
                        {
                            continue;
                        }

                        int warehouseQty = await _productService.GetTotalQty(c.ProductId);
                        if(warehouseQty <= 0)
                        {
                            continue;
                        }

                        UserCart? userCart = await _cartService.GetUserCartItem(userId, c.ProductId);
                        if (c.Quantity > warehouseQty)
                        {
                            c.Quantity = warehouseQty;
                        }
                        if(c.Quantity <= 0)
                        {
                            c.Quantity = 1;
                        }

                        UserCart cartToUpdate = new UserCart
                        {
                            ProductId = c.ProductId,
                            Quantity = c.Quantity,
                            UserId = userId,
                        };

                        if (userCart == null)
                        {
                            await _cartService.AddToCart(cartToUpdate);
                        }
                        else
                        {
                            await _cartService.UpdateQuantity(cartToUpdate, c.Quantity);
                        }
                    }

                    DeleteCookieCart();
                }

                cart = await _cartService.GetUserCart(userId);

                List<UserCart> cartToDelete = new List<UserCart>();
                foreach (UserCart c in cart)
                {
                    if(await _productService.CheckOutOfStock(c.ProductId))
                    {
                        cartToDelete.Add(c);
                    }
                }

                if(cartToDelete.Count > 0)
                {
                    await _cartService.RemoveListCart(cartToDelete);
                    cart = await _cartService.GetUserCart(userId);
                }

                User? user = await _userService.GetUserById(userId);
                ViewBag.User = user;
            }
            else
            {
                List<UserCart> _cart = new List<UserCart>();
                foreach(CartVM c in cartFromCookie)
                {
                    if(c.ProductId == null)
                    {
                        cartFromCookie.Remove(c);
                        continue;
                    }

                    Product? product = await _productService.GetProductWithBasicInfo(c.ProductId);

                    if(product == null)
                    {
                        cartFromCookie.Remove(c);
                        continue;
                    }

                    int warehouseQty = product.WarehouseProducts.Sum(p => p.Quantity);
                    if (warehouseQty <= 0)
                    {
                        cartFromCookie.Remove(c);
                        continue;
                    }

                    if (c.Quantity > warehouseQty)
                    {
                        c.Quantity = warehouseQty;
                    }
                    if(c.Quantity <= 0)
                    {
                        c.Quantity = 1;
                    }

                    _cart.Add(new UserCart
                    {
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        Product = product
                    });
                }

                cart = _cart;
                SaveCartToCookie(cartFromCookie);
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string id)
        {
            if(id == null || await _productService.CheckOutOfStock(id))
            {
                return BadRequest();
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");
                if(userId == null)
                {
                    return BadRequest();
                }

                UserCart? cart = await _cartService.GetUserCartItem(userId, id);

                if(cart == null)
                {
                    return Ok(new ApiResponse
                    {
                        Status = await _cartService.AddToCart(new UserCart
                        {
                            ProductId = id,
                            UserId = userId,
                            Quantity = 1
                        })
                    });
                }
                else
                {
                    int warehouseQty = await _productService.GetTotalQty(id);
                    int qty = cart.Quantity + 1;
                    if(qty > warehouseQty)
                    {
                        qty = warehouseQty;
                    }

                    return Ok(new ApiResponse
                    {
                        Status = await _cartService.UpdateQuantity(cart, qty)
                    });
                }
            }
            else
            {
                List<CartVM> cartFromCookie = GetCartFromCookie();

                //----------
                CartVM? cart = cartFromCookie.FirstOrDefault(t => t.ProductId == id);

                if (cart == null)
                {
                    cartFromCookie.Add(new CartVM
                    {
                        ProductId = id,
                        Quantity = 1
                    });
                }
                else
                {
                    int warehouseQty = await _productService.GetTotalQty(id);
                    cart.Quantity += 1;
                    if (cart.Quantity >= warehouseQty)
                    {
                        cart.Quantity = warehouseQty;
                    }
                }

                SaveCartToCookie(cartFromCookie);

                return Ok(new ApiResponse
                {
                    Status = true
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateQuantity(string id, string type, int qty)
        {
            if(id == null)
            {
                return BadRequest();
            }

            Product product = await _productService.GetProductWithBasicInfo(id) ?? new Product();
            int warehouseQty = await _productService.GetTotalQty(id);
            string message = "";
            int updatedQty = qty;
            decimal totalPrice = 0;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return BadRequest();
                }

                UserCart? cart = await _cartService.GetUserCartItem(userId, id);
                if (cart == null)
                {
                    return BadRequest();
                }


                if(warehouseQty <= 0)
                {
                    await _cartService.RemoveFromCart(cart);
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = $"Sản phẩm <strong class=\"text-danger\">{product.ProductName}</strong> đã hết hàng"
                    });
                }

                cart.Quantity = UpdateCartItemQuantity(type, cart.Quantity, qty);

                if (cart.Quantity > warehouseQty)
                {
                    cart.Quantity = warehouseQty;
                    message = $"<strong class=\"text-danger\">{product.ProductName}</strong> chỉ còn lại {warehouseQty} sản phẩm";
                }

                await _cartService.UpdateQuantity(cart, cart.Quantity);

                IEnumerable<UserCart> userCart = await _cartService.GetUserCart(userId);
                totalPrice = userCart.Sum(c => c.Quantity * c.Product.Price);
                updatedQty = cart.Quantity;
            }
            else
            {
                List<CartVM> cartFromCookie = GetCartFromCookie();
                CartVM? cart = cartFromCookie.FirstOrDefault(c => c.ProductId == id);

                if (cart == null)
                {
                    return BadRequest();
                }

                if (warehouseQty <= 0)
                {
                    cartFromCookie.Remove(cart);
                    return Ok(new ApiResponse
                    {
                        Status = false,
                        Message = $"Sản phẩm <strong class=\"text-danger\">{product.ProductName}</strong> đã hết hàng"
                    });
                }

                cart.Quantity = UpdateCartItemQuantity(type, cart.Quantity, qty);

                if (cart.Quantity > warehouseQty)
                {
                    cart.Quantity = warehouseQty;
                    message = $"<strong class=\"text-danger\">{product.ProductName}</strong> chỉ còn lại {warehouseQty} sản phẩm";
                }

                foreach(CartVM c in cartFromCookie)
                {
                    if (c.ProductId == null)
                    {
                        cartFromCookie.Remove(c);
                        continue;
                    }

                    Product? p = await _productService.GetProductWithBasicInfo(c.ProductId);
                    totalPrice += c.Quantity * p.Price;
                }

                updatedQty = cart.Quantity;
                SaveCartToCookie(cartFromCookie);
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Message = message,
                Data = new { Quantity = updatedQty, TotalPrice = totalPrice }
            });
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return Redirect("/cart");
            }

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return Redirect("/cart");
                }

                UserCart? cart = await _cartService.GetUserCartItem(userId, id);
                if(cart == null)
                {
                    return Redirect("/cart");
                }
                await _cartService.RemoveFromCart(cart);
            }
            else
            {
                List<CartVM> cartFromCookie = GetCartFromCookie();
                CartVM? cart = cartFromCookie.FirstOrDefault(c => c.ProductId == id);

                if(cart == null)
                {
                    return BadRequest();
                }

                cartFromCookie.Remove(cart);
                SaveCartToCookie(cartFromCookie);
            }


            return Redirect("/cart");
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                string? userId = User.FindFirstValue("Id");
                if(userId == null)
                {
                    return BadRequest();
                }

                IEnumerable<UserCart> cart = await _cartService.GetUserCart(userId);

                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = cart.Count()
                });
            } else
            {
                IEnumerable<CartVM> cart = GetCartFromCookie();
                return Ok(new ApiResponse
                {
                    Status = true,
                    Data = cart.Count()
                });
            }
        }
    }
}
