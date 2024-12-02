using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.ViewModels;
using STech.Services;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public StatisticsController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpGet("last-six-month-order-summary")]
        public async Task<IActionResult> GetRevenue()
        {
            IEnumerable<MonthlyOrderSummary> data = await _orderService.GetLastSixMonthSummary();

            return Ok(new ApiResponse
            {
                Status = true,
                Data = data
            });
        }
    }
}
