using Microsoft.AspNetCore.Mvc;
using STech.Contants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserService _userService;

        private readonly int _pageSize = 30;

        public EmployeesController(IEmployeeService employeeService, 
            IAuthorizationService authorizationService,
            IUserService userService)
        {
            _employeeService = employeeService;
            _authorizationService = authorizationService;
            _userService = userService;
        }

        [AdminAuthorize(Code = Functions.ViewStaffs)]
        public async Task<IActionResult> Index(string? sort_by, int page = 1)
        {
            var employees = await _employeeService.GetEmployees(page, _pageSize);

            ViewBag.ActiveSidebar = "employees";
            return View(employees);
        }

        [AdminAuthorize(Code = Functions.CreateStaff)]
        public async Task<IActionResult> Create()
        {
            IEnumerable<UserGroup> userGroups = await _authorizationService.GetUserGroups();

            ViewBag.ActiveSidebar = "employees";
            return View("CreateUpdate", 
                new Tuple<EmployeeVM, IEnumerable<UserGroup>, bool>(new EmployeeVM(), userGroups, false));
        }

        [AdminAuthorize(Code = Functions.EditStaff)]
        public async Task<IActionResult> Update(string id)
        {
            Employee? employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return RedirectToAction(nameof(Index));
            }

            User? user = await _userService.GetEmployeeUser(id);

            IEnumerable<UserGroup> userGroups = await _authorizationService.GetUserGroups();
            EmployeeVM employeeVM = new EmployeeVM
            {
                EmployeeName = employee.EmployeeName,
                Phone = employee.Phone,
                Email = employee.Email,
                Dob = employee.Dob?.ToDateTime(TimeOnly.MinValue),
                GroupId = user?.GroupId,
                Gender = employee.Gender,
                Address = employee.Address,
                WardCode = employee.WardCode,
                DistrictCode = employee.DistrictCode,
                ProvinceCode = employee.ProvinceCode,
            };

            ViewBag.ActiveSidebar = "employees";
            return View("CreateUpdate",
                new Tuple<EmployeeVM, IEnumerable<UserGroup>, bool>(employeeVM, userGroups, true));
        }
    }
}
