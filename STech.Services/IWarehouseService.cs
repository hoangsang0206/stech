using STech.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistance(double latitude, double longtitude);
        Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistanceWithProduct(double? latitude, double? longtitude);
        Task<Warehouse?> GetNearestWarehouse(double latitude, double longtitude);
        Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId);
        Task<bool> CreateWarehouseExports(IEnumerable<WarehouseExport> warehouseExports);
        Task<bool> SubtractProductQuantity(IEnumerable<WarehouseExport> warehouseExports);
    }
}
