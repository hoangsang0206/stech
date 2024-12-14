using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using System.Security.Claims;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        [AdminAuthorize]
        public async Task<IActionResult> GetMyProfile()
        {
            string? userId = User.FindFirstValue("Id");

            if (userId == null)
            {
                return Unauthorized();
            }

            Employee? emp = await _employeeService.GetEmployeeByUserId(userId);

            return Ok(new ApiResponse
            {
                Status = true,
                Message = emp == null ? "Không tìm thấy thông tin" : "",
                Data = emp
            });
        }
    }
}
