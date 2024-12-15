using Microsoft.AspNetCore.Mvc;
using STech.Contants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly int _itemsPerPage = 30;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [AdminAuthorize(Code = Functions.ViewInvoices)]
        public async Task<IActionResult> Index(int page = 1, string? filter_by = "unaccepted", string? sort_by = null)
        {
            PagedList<Invoice> invoices = await _orderService.GetInvoices(page, _itemsPerPage, filter_by, sort_by);

            ViewBag.ActivePageNav = filter_by;
            ViewBag.ActiveSidebar = "orders";

            return View(invoices);
        }

        [Route("admin/orders/search/{query}")]
        [AdminAuthorize(Code = Functions.ViewInvoices)]
        public async Task<IActionResult> SearchOrders(string query)
        {
            PagedList<Invoice> invoices = await _orderService.SearchInvoices(query, 1, _itemsPerPage);

            ViewBag.ActiveSidebar = "orders";
            ViewBag.SearchValue = query;
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;

            return View("Index", invoices);
        }

        [AdminAuthorize(Code = Functions.CreateInvoice)]
        public IActionResult Create()
        {
            ViewBag.ActiveSidebar = "orders";
            return View();
        }

        [AdminAuthorize(Code = Functions.ViewInvoices)]
        public async Task<IActionResult> Detail(string id)
        {
            Invoice? invoice = await _orderService.GetInvoiceWithAllDetails(id);
            if (invoice == null)
            {
                return NotFound();
            }

            ViewBag.ActiveSidebar = "orders";
            return View(invoice);
        }
    }
}
