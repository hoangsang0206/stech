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

        [HttpGet("cities")]
        public IActionResult GetCities()
        {
            IEnumerable<AddressVM.City> cities = _addressService.Address.Cities;
            return Ok(cities);
        }

        [HttpGet("districts/{cityCode}")]
        public IActionResult GetDistricts(string cityCode)
        {
            AddressVM.City? city = _addressService.Address.Cities.FirstOrDefault(c => c.code == cityCode);
            if(city == null)
            {
                return BadRequest();
            }

            return Ok(city.districts);
        }

        [HttpGet("wards/{districtCode}")]
        public IActionResult GetWards(string districtCode)
        {
            AddressVM.District? district = _addressService.Address.Districts.FirstOrDefault(d => d.code == districtCode);
            if(district == null)
            {
                return BadRequest();
            }   

            return Ok(district.wards);
        }
    }
}
