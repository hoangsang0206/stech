using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(int page = 1, string? filter_by = "unaccepted", string? sort_by = null)
        {
            var (invoices, totalPage ) = await _orderService.GetInvoices(page, filter_by, sort_by);

            ViewBag.ActivePageNav = filter_by;
            ViewBag.ActiveSidebar = "orders";

            return View(invoices);
        }
    }
}
