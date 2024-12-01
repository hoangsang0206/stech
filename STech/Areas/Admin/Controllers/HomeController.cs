using Microsoft.AspNetCore.Mvc;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), AdminAuthorize]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public HomeController(IProductService productService, IUserService userService, IOrderService orderService)
        {
            _productService = productService;
            _userService = userService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            AdminHomePageData data = new AdminHomePageData
            {
                ProductStatistic = new ProductStatistic
                {
                    TotalProducts = await _productService.GetTotalProducts(),
                    CurrentMonthAdded = await _productService.GetMonthAdded(DateTime.Now.Month),
                },
                OrderStatistic = new OrderStatistic
                {
                    TotalOrders = await _orderService.GetTotalOrders(),
                    TotalRevenue = await _orderService.GetTotalRevenue(),
                    LastMonthOrders = await _orderService.GetMonthOrders(DateTime.Now.AddMonths(-1).Month),
                    CurrentMonthOrders = await _orderService.GetMonthOrders(DateTime.Now.Month),
                    LastMonthRevenue = await _orderService.GetMonthRevenue(DateTime.Now.AddMonths(-1).Month),
                    CurrentMonthRevenue = await _orderService.GetMonthRevenue(DateTime.Now.Month),
                },
                TopSellingProducts = await _productService.GetBestSellingProducts(10),
                RecentOrders = await _orderService.GetRecentOrders(10),
                TopUsers = await _userService.GetTopUsers(10),
            };

            ViewBag.ActiveSidebar = "home";
            return View(data);
        }
    }
}
