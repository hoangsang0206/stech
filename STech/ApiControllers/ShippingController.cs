using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Services.Services;
using STech.Utils;
using Stripe;

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

        private double CalculateFee(double distance)
        {
            double fee = distance switch
            {
                <= 5 => 15000,
                <= 15 => 20000,
                <= 30 => 25000,
                <= 50 => 35000,
                <= 100 => 40000,
                _ => 50000,
            };


            return Math.Floor(fee);
        }

        [HttpGet("fee")]
        public async Task<IActionResult> CalculateShippingFee(string city, string district, string ward)
        {
            Warehouse? warehouse = await _warehouseService.GetOnlineWarehouse();
            if (warehouse == null)
            {
                return BadRequest();
            }

            AddressVM.City _city = _addressService.Cities.FirstOrDefault(c => c.code == city) ?? new AddressVM.City();
            AddressVM.District _district = _city.districts.FirstOrDefault(c => c.code == district) ?? new AddressVM.District();
            AddressVM.Ward _ward = _district.wards.FirstOrDefault(c => c.code == ward) ?? new AddressVM.Ward();

            AddressVM.City whCity = _addressService.Cities.FirstOrDefault(c => c.code == warehouse.ProvinceCode) ?? new AddressVM.City();
            AddressVM.District whDistrict = whCity.districts.FirstOrDefault(c => c.code == warehouse.DistrictCode) ?? new AddressVM.District();
            AddressVM.Ward whWard = whDistrict.wards.FirstOrDefault(c => c.code == warehouse.WardCode) ?? new AddressVM.Ward();

            var (latitude, longtitude) = await _geocodioService.GetLocation(_city.name_with_type, _district.name_with_type, _ward.name_with_type);
            var (whLat, whLong) = await _geocodioService.GetLocation(whCity.name_with_type, whDistrict.name_with_type, whWard.name_with_type);

            if (!latitude.HasValue || !longtitude.HasValue || !whLat.HasValue || !whLong.HasValue)
            {
                return NotFound();
            }

            double distance = GeocodioUtils.CalculateDistance(latitude.Value, longtitude.Value, whLat.Value, whLong.Value);
            double fee = CalculateFee(distance);

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
