using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly StechDbContext _context;

        public WarehouseService(StechDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouse>> GetWarehouses()
        {
            return await _context.Warehouses.ToListAsync();
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistance(double latitude, double longtitude)
        {
            IEnumerable<Warehouse> warehouses = await GetWarehouses();
            return warehouses.OrderByDistance(latitude, longtitude);
        }

        public async Task<IEnumerable<Warehouse>> GetWarehousesOrderByDistanceWithProduct(double? latitude, double? longtitude)
        {
            IEnumerable<Warehouse> warehouses = warehouses = await _context.Warehouses
                .Include(w => w.WarehouseProducts)
                .ToListAsync();

            if (latitude == null || longtitude == null)
            {
                return warehouses.OrderByDescending(w => w.WarehouseProducts.Count);
            }

            return warehouses.OrderByDistance(latitude.Value, longtitude.Value);
        }

        public async Task<Warehouse?> GetNearestWarehouse(double latitude, double longtitude)
        {
            IEnumerable<Warehouse> warehouses = await GetWarehousesOrderByDistance(latitude, longtitude);
            Warehouse? warehouse = warehouses.FirstOrDefault();

            return warehouse;
        }

        public async Task<Warehouse?> GetWarehouseById(string warehouseId)
        {
            return await _context.Warehouses.FindAsync(warehouseId);
        }

        public async Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId)
        {
            return await _context.WarehouseProducts
                .Where(p => p.ProductId == productId)
                .Include(p => p.Warehouse)
                .ToListAsync();
        }

        public async Task<bool> CreateWarehouseExports(IEnumerable<WarehouseExport> warehouseExports)
        {
            IEnumerable<WarehouseExportDetail> wheDetails = warehouseExports.SelectMany(t => t.WarehouseExportDetails).ToList();
            warehouseExports.ToList().ForEach(t => t.WarehouseExportDetails = new List<WarehouseExportDetail>());

            await _context.WarehouseExports.AddRangeAsync(warehouseExports);
            await _context.SaveChangesAsync();

            await _context.WarehouseExportDetails.AddRangeAsync(wheDetails);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> SubtractProductQuantity(IEnumerable<WarehouseExport> warehouseExports)
        {
            foreach (WarehouseExport wE in warehouseExports)
            {
                foreach(WarehouseExportDetail detail in wE.WarehouseExportDetails)
                {
                    WarehouseProduct? whP = await _context.WarehouseProducts
                        .Where(wp => wp.ProductId == detail.ProductId && wp.WarehouseId == wE.WarehouseId).FirstOrDefaultAsync();

                    if (whP != null)
                    {
                        whP.Quantity -= detail.RequestedQuantity;
                        _context.WarehouseProducts.Update(whP);
                    }
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
