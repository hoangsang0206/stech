using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using STech.Data.ViewModels;
using STech.Services.Services;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly AddressService _addressService;

        public AddressController(AddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("cities"), Authorize]
        public IActionResult GetCities()
        {
            IEnumerable<AddressVM.City> cities = _addressService.Cities;
            return Ok(cities);
        }

        [HttpGet("districts/{cityCode}"), Authorize]
        public IActionResult GetDistricts(string cityCode)
        {
            IEnumerable<AddressVM.District>? districts = _addressService.Districts;
            districts = districts?.Where(d => d.parent_code == cityCode);
            return Ok(districts);
        }

        [HttpGet("wards/{districtCode}"), Authorize]
        public IActionResult GetWards(string districtCode)
        {
            IEnumerable<AddressVM.Ward>? wards = _addressService.Wards;
            wards = wards?.Where(w => w.parent_code == districtCode);
            return Ok(wards);
        }
    }
}
