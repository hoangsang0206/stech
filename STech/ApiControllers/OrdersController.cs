using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using System.Security.Claims;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("all"), Authorize]
        public async Task<IActionResult> GetOrders()
        {
            string? userId = User.FindFirstValue("Id");

            if (userId == null)
            {
                return Unauthorized();
            }

            IEnumerable<Invoice> invoices = await _orderService.GetUserInvoices(userId);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoices
            });
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckOrder([FromQuery] string oId, string phone)
        {
            Invoice? invoice = await _orderService.GetInvoice(oId, phone);

            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoice
            });
        }
    }
}
