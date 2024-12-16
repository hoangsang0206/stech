using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers;

[Area("Admin")]
public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;
    
    private readonly int _itemsPerPage = 30;
    
    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    [AdminAuthorize(Code = Functions.ViewCustomers)]
    public async Task<IActionResult> Index(string? sort_by, string? filter_by, int page= 1)
    {
        PagedList<Customer> customers = await _customerService.GetCustomers(page,_itemsPerPage, filter_by, sort_by);
        
        ViewBag.ActiveSidebar = "customers";
        return View(customers);
    }
}