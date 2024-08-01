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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("userorders"), Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] string? type)
        {
            string? userId = User.FindFirstValue("Id");

            if (userId == null)
            {
                return Unauthorized();
            }

            IEnumerable<Invoice> invoices = await _orderService.GetUserInvoices(userId);

            switch(type)
            {
                case "completed":
                    invoices = invoices.Where(i => i.IsCompleted == true);
                    break;
                case "uncompleted":
                    invoices = invoices.Where(i => i.IsCompleted == false && i.IsCancelled == false);
                    break;
                case "cancelled":
                    invoices = invoices.Where(i => i.IsCancelled == true);
                    break;
                case "paid":
                    invoices = invoices.Where(i => i.PaymentStatus == PaymentContants.Paid);
                    break;
                case "unpaid":
                    invoices = invoices.Where(i => i.PaymentStatus == PaymentContants.UnPaid);
                    break;
                default:
                    break;
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = invoices
            });
        }

        [HttpGet("one")]
        public async Task<IActionResult> CheckOrder([FromQuery] string oId, string phone)
        {
            Invoice? invoice = await _orderService.GetInvoiceWithDetails(oId, phone);

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
