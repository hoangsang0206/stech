using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {


        [HttpGet("revenue")]
        public IActionResult GetRevenue()
        {
            return Ok();
        }
    }
}
