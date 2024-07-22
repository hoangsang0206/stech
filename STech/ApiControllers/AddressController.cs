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
            AddressVM.City? city = _addressService.Cities.FirstOrDefault(c => c.code == cityCode);
            if(city == null)
            {
                return BadRequest();
            }

            return Ok(city.districts);
        }

        [HttpGet("wards/{districtCode}"), Authorize]
        public IActionResult GetWards(string districtCode)
        {
            AddressVM.District? district = _addressService.Districts.FirstOrDefault(d => d.code == districtCode);
            if(district == null)
            {
                return BadRequest();
            }   

            return Ok(district.wards);
        }
    }
}
