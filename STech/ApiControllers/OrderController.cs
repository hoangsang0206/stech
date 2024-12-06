using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Constants;
using System.Security.Claims;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IWarehouseService _warehouseService;

        public OrderController(IProductService productService, ICartService cartService, 
            IOrderService orderService, IWarehouseService warehouseService)
        {
            _productService = productService;
            _cartService = cartService;
            _orderService = orderService;
            _warehouseService = warehouseService;
        }

        [Authorize, HttpGet("check")]
        public async Task<IActionResult> CheckQuantity([FromQuery] string? pId)
        {
            string? message = null;

            if(pId != null)
            {
                Product p = await _productService.GetProductWithBasicInfo(pId) ?? new Product();
                int qty = p.WarehouseProducts.Sum(wp => wp.Quantity);

                if(qty <= 0)
                {
                    message += $"<li>Sản phẩm <span class=\"fw-bold text-primary\">{p.ProductName}</span> đã hết hàng</li>";
                }
            }
            else 
            {
                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return BadRequest();
                }

                IEnumerable<UserCart> cart = await _cartService.GetUserCart(userId);

                foreach (UserCart c in cart)
                {
                    Product p = await _productService.GetProductWithBasicInfo(c.ProductId) ?? new Product();

                    int qty = p.WarehouseProducts.Sum(wp => wp.Quantity);

                    if (qty < c.Quantity)
                    {
                        message += $"<li>Số lượng sản phẩm <span class=\"fw-bold text-primary\">{p.ProductName}</span> chỉ còn lại <span class=\"fw-bold text-primary\">{qty}</span></li>";
                    }
                    else if (qty == 0)
                    {
                        message += $"<li>Sản phẩm <span class=\"fw-bold text-primary\">{p.ProductName}</span> đã hết hàng</li>";
                    }
                }

            }

            return Ok(new ApiResponse
            {
                Status = message == null ? true : false,
                Data = message
            });
        }

        [Authorize, HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(string id)
        {
            string? userId = User.FindFirstValue("Id");
            if (userId == null)
            {
                return BadRequest();
            }

            Invoice? invoice = await _orderService.GetUserInvoice(id, userId);

            if(invoice == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            if (invoice.PaymentStatus == PaymentContants.Paid)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không thể hủy đơn hàng đã thanh toán"
                });
            }

            bool result = await _orderService.CancelOrder(userId, id);

            if(result)
            {
                await _warehouseService.CancelInvoiceWarehouseExports(id);
            }

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Hủy đơn hàng thành công" : "Không thể hủy đơn hàng này"
            });
        }
    }
}
