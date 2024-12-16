using Microsoft.AspNetCore.Mvc;
using STech.Constants;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using STech.Services.Services;

namespace STech.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly AddressService _addressService;

        public CustomersController(ICustomerService customerService, AddressService addressService)
        {
            _customerService = customerService;
            _addressService = addressService;
        }

        [HttpGet("search/phone/{phone}")]
        [AdminAuthorize(Code = Functions.ViewCustomers)]
        public async Task<IActionResult> SearchByPhone(string phone)
        {
            IEnumerable<Customer> customers = await _customerService.SearchCustomers(phone);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = customers
            });
        }

        [HttpGet("1/{id}")]
        [AdminAuthorize(Code = Functions.ViewCustomers)]
        public async Task<IActionResult> GetById(string id)
        {
            Customer? customer = await _customerService.GetCustomerById(id);

            if (customer == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    Message = "Không tìm thấy khách hàng này"
                });
            }

            return Ok(new ApiResponse
            {
                Status = true,
                Data = customer
            });
        }

        [HttpPost("create")]
        [AdminAuthorize(Code = Functions.CreateCustomer)]
        public async Task<IActionResult> CreateCustomer(CustomerVM customer)
        {
            if (ModelState.IsValid)
            {
                AddressVM address = new AddressVM();
                address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == customer.CityCode);
                address._District = address._City?.districts.FirstOrDefault(c => c.code == customer.DistrictCode);
                address._Ward = address._District?.wards.FirstOrDefault(c => c.code == customer.WardCode);

                Customer _customer = new Customer()
                {
                    CustomerName = customer.CustomerName,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Gender = customer.Gender,
                    Dob = customer.Dob,

                    ProvinceCode = customer.CityCode,
                    Province = address._City?.name_with_type ?? "",
                    DistrictCode = customer.DistrictCode,
                    District = address._District?.name_with_type ?? "",
                    WardCode = customer.WardCode,
                    Ward = address._Ward?.name_with_type ?? "",
                    Address = customer.Address
                };

                bool result = await _customerService.CreateCustomer(_customer);

                return Ok(new ApiResponse
                {
                    Status = result,
                    Message = result ? "Thêm khách hàng thành công" : "Không thể thêm khách hàng"
                });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("update")]
        [AdminAuthorize(Code = Functions.EditCustomer)]
        public async Task<IActionResult> UpdateCustomer(CustomerVM customer)
        {
            if(string.IsNullOrEmpty(customer.CustomerId))
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                AddressVM address = new AddressVM();
                address._City = _addressService.Address.Cities.FirstOrDefault(c => c.code == customer.CityCode);
                address._District = address._City?.districts.FirstOrDefault(c => c.code == customer.DistrictCode);
                address._Ward = address._District?.wards.FirstOrDefault(c => c.code == customer.WardCode);

                Customer? _customer = await _customerService.GetCustomerById(customer.CustomerId);
                if(_customer != null)
                {
                    _customer.CustomerName = customer.CustomerName;
                    _customer.Phone = customer.Phone;
                    _customer.Email = customer.Email;
                    _customer.Gender = customer.Gender;
                    _customer.Dob = customer.Dob;

                    _customer.Address = customer.Address;
                    _customer.Ward = address._Ward?.name_with_type ?? "";
                    _customer.WardCode = customer.WardCode;
                    _customer.District = address._District?.name_with_type ?? "";
                    _customer.DistrictCode = customer.DistrictCode;    
                    _customer.Province = address._City?.name_with_type ?? "";
                    _customer.ProvinceCode = customer.CityCode;

                    bool result = await _customerService.UpdateCustomer(_customer);

                    return Ok(new ApiResponse
                    {
                        Status = result,
                        Message = result ? "Cập nhật khách hàng thành công" : "Không thể cập nhật khách hàng"
                    });
                }
            }

            return Ok(new ApiResponse
            {
                Status = false,
                Message = "Không tìm thấy khách hàng này"
            });
        }

        [HttpDelete("{id}")]
        [AdminAuthorize(Code = Functions.DeleteCustomer)]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            bool result = await _customerService.DeleteCustomer(id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa khách hàng thành công" : "Không thể xóa khách hàng"
            });
        }
    }
}
