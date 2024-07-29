using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Services;
using STech.Services.Utils;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IGeocodioService _geocodioService;
        private readonly IWarehouseService _warehouseService;
        private readonly AddressService _addressService;

        public ShippingController(IGeocodioService geocodioService, IWarehouseService warehouseService, AddressService addressService)
        {
            _geocodioService = geocodioService;
            _warehouseService = warehouseService;
            _addressService = addressService;
        }

        [HttpGet("fee")]
        public async Task<IActionResult> CalculateShippingFee(string city, string district, string ward)
        {
            AddressVM.City _city = _addressService.Address.Cities.FirstOrDefault(c => c.code == city) ?? new AddressVM.City();
            AddressVM.District _district = _city.districts.FirstOrDefault(c => c.code == district) ?? new AddressVM.District();
            AddressVM.Ward _ward = _district.wards.FirstOrDefault(c => c.code == ward) ?? new AddressVM.Ward();

            var (latitude, longtitude) = await _geocodioService.GetLocation(_city.name_with_type, _district.name_with_type, _ward.name_with_type);

            if (!latitude.HasValue || !longtitude.HasValue)
            {
                return NotFound();
            }

            Warehouse? warehouse = await _warehouseService.GetNearestWarehouse(latitude.Value, longtitude.Value);
            if (warehouse == null)
            {
                return BadRequest();
            }

            double distance = GeocodioUtils.CalculateDistance(latitude.Value, longtitude.Value, Convert.ToDouble(warehouse.Latitude), Convert.ToDouble(warehouse.Longtitude));
            double fee = GeocodioUtils.CalculateFee(distance);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    Fee = fee
                }
            });
        }
    }
}
