using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace STech.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;

        public CartController(ICartService cartService, IProductService productService, IUserService userService)
        {
            _cartService = cartService;
            _productService = productService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<UserCart> cart = new List<UserCart>();
            List<CartVM> cartFromCookie = CartUtils.GetCartFromCookie(Request);

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

                    CartUtils.DeleteCookieCart(Response);
                }

                cart = await _cartService.GetUserCart(userId);

                List<UserCart> cartToDelete = new List<UserCart>();
                foreach (UserCart c in cart)
                {
                    int qty = await _productService.GetTotalQty(c.ProductId);
                    if(qty <= 0)
                    {
                        cartToDelete.Add(c);
                    } 
                    else if(c.Quantity > qty)
                    {
                        await _cartService.UpdateQuantity(c, qty);
                    }
                }

                if(cartToDelete.Count > 0)
                {
                    await _cartService.RemoveListCart(cartToDelete);
                }

                cart = await _cartService.GetUserCart(userId);
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
                CartUtils.SaveCartToCookie(Response, cartFromCookie);
            }

            IEnumerable<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Giỏ hàng", "")
            };

            return View(new Tuple<IEnumerable<UserCart>, IEnumerable<Breadcrumb>>(cart, breadcrumbs));
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
                List<CartVM> cartFromCookie = CartUtils.GetCartFromCookie(Request);
                CartVM? cart = cartFromCookie.FirstOrDefault(c => c.ProductId == id);

                if(cart == null)
                {
                    return BadRequest();
                }

                cartFromCookie.Remove(cart);
                CartUtils.SaveCartToCookie(Response, cartFromCookie);
            }


            return Redirect("/cart");
        }
    }
}
