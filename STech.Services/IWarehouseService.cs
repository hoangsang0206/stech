using STech.Data.Models;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<Warehouse>> GetWarehouses();
        Task<IEnumerable<Warehouse>> GetWarehousesWithMostRecentImportAndExport();
        Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistance(double latitude, double longtitude);
        Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistanceWithProduct(double? latitude, double? longtitude);
        Task<Warehouse?> GetNearestWarehouse(double latitude, double longtitude);
        Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId);
        Task<Warehouse?> GetWarehouseById(string warehouseId);

        Task<bool> CreateWarehouse(Warehouse warehouse);
        Task<bool> UpdateWarehouse(Warehouse warehouse);

        Task<bool> CreateWarehouseExports(IEnumerable<WarehouseExport> warehouseExports);
        Task<bool> SubtractProductQuantity(IEnumerable<WarehouseExport> warehouseExports);

        Task<bool> CancelInvoiceWarehouseExports(string invoiceId);


        Task<bool> CreateWarehouseImport(WarehouseImportVM import, string employeeId);
    }
}
