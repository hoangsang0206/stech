using Microsoft.EntityFrameworkCore;
using STech.Data.Models;
using STech.Data.ViewModels;
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

        public async Task<IEnumerable<Warehouse>> GetWarehousesWithMostRecentImportAndExport()
        {
            return await _context.Warehouses
                .Select(w => new Warehouse
                {
                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.WarehouseName,
                    Address = w.Address,
                    Ward = w.Ward,
                    Province = w.Province,
                    District = w.District,
                    WarehouseImports = w.WarehouseImports.OrderByDescending(i => i.DateImport).Take(1).ToList(),
                    WarehouseExports = w.WarehouseExports.OrderByDescending(e => e.DateExport).Take(1).ToList()
                })
                .ToListAsync();
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
        
        public async Task<Warehouse?> GetWarehouseByIdWithStockInfo(string warehouseId)
        {
            return await _context.Warehouses
                .Include(w => w.WarehouseProducts)
                .Include(w => w.WarehouseExports)
                .Include(w => w.WarehouseImports)
                .FirstOrDefaultAsync(w => w.WarehouseId == warehouseId);
        }

        public async Task<IEnumerable<WarehouseProduct>> GetWarehouseProducts(string productId)
        {
            return await _context.WarehouseProducts
                .Where(p => p.ProductId == productId)
                .Include(p => p.Warehouse)
                .ToListAsync();
        }

        public async Task<bool> CreateWarehouse(Warehouse warehouse)
        {
            Warehouse? existedWarehouse = await GetWarehouseById(warehouse.WarehouseId);

            if (existedWarehouse != null)
            {
                return false;
            }

            await _context.Warehouses.AddAsync(warehouse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateWarehouse(Warehouse warehouse)
        {
            Warehouse? existedWarehouse = await GetWarehouseById(warehouse.WarehouseId);

            if (existedWarehouse == null)
            {
                return false;
            }

            existedWarehouse.WarehouseName = warehouse.WarehouseName;
            existedWarehouse.Address = warehouse.Address;
            existedWarehouse.Ward = warehouse.Ward;
            existedWarehouse.WardCode = warehouse.WardCode;
            existedWarehouse.District = warehouse.District;
            existedWarehouse.DistrictCode = warehouse.DistrictCode;
            existedWarehouse.Province = warehouse.Province;
            existedWarehouse.ProvinceCode = warehouse.ProvinceCode;
            existedWarehouse.Latitude = warehouse.Latitude;
            existedWarehouse.Longtitude = warehouse.Longtitude;

            return await _context.SaveChangesAsync() > 0;
        }
        
        public async Task<bool> DeleteWarehouse(string warehouseId)
        {
            Warehouse? warehouse = await GetWarehouseByIdWithStockInfo(warehouseId);

            if (warehouse == null || warehouse.WarehouseProducts.Any() 
                || warehouse.WarehouseExports.Any() || warehouse.WarehouseImports.Any())
            {
                return false;
            }

            _context.Warehouses.Remove(warehouse);
            return await _context.SaveChangesAsync() > 0;
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
                foreach (WarehouseExportDetail detail in wE.WarehouseExportDetails)
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

        public async Task<bool> CancelInvoiceWarehouseExports(string invoiceId)
        {
            Invoice? invoice = await _context.Invoices
                .Where(i => i.InvoiceId == invoiceId)
                .Include(i => i.WarehouseExports)
                .ThenInclude(we => we.WarehouseExportDetails)
                .FirstOrDefaultAsync();

            if (invoice == null)
            {
                return false;
            }

            foreach (WarehouseExport export in invoice.WarehouseExports)
            {
                foreach (WarehouseExportDetail detail in export.WarehouseExportDetails)
                {
                    WarehouseProduct? whProduct = await _context.WarehouseProducts
                        .FirstOrDefaultAsync(wp => wp.ProductId == detail.ProductId && wp.WarehouseId == export.WarehouseId);

                    if (whProduct != null)
                    {
                        whProduct.Quantity += detail.RequestedQuantity;
                    }
                }

                export.Note = "Đã hủy theo hóa đơn";
            }

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> CreateWarehouseImport(WarehouseImportVM import, string employeeId)
        {
            WarehouseImport _import = new WarehouseImport
            {
                WarehouseId = import.WarehouseId,
                SupplierId = import.SupplierId,
                DateImport = DateTime.Now,
                EmployeeId = employeeId,
                Note = import.Note,
                DateCreate = DateTime.Now,
            };

            _import.WarehouseImportDetails = import.WarehouseImportDetails.Select(d => new WarehouseImportDetail
            {
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
            }).ToList();

            _import.WarehouseImportHistories = import.WarehouseImportDetails.Select(d => new WarehouseImportHistory
            {
                HistoryId = Guid.NewGuid().ToString(),
                ProductId = d.ProductId,
                Quantity = d.Quantity,
                BatchNumber = d.BatchNumber,
                ImportDate = _import.DateImport ?? DateTime.Now
                
            }).ToList();

            await _context.WarehouseImports.AddAsync(_import);
            bool result = await _context.SaveChangesAsync() > 0;

            if (result)
            {
                foreach (var detail in import.WarehouseImportDetails)
                {
                    WarehouseProduct? whProduct = await _context.WarehouseProducts
                        .FirstOrDefaultAsync(wp => wp.ProductId == detail.ProductId && wp.WarehouseId == import.WarehouseId);

                    if (whProduct != null)
                    {
                        whProduct.Quantity += detail.Quantity;
                    }
                    else
                    {
                        WarehouseProduct newWhProduct = new WarehouseProduct
                        {
                            ProductId = detail.ProductId,
                            WarehouseId = import.WarehouseId,
                            Quantity = detail.Quantity
                        };

                        await _context.WarehouseProducts.AddAsync(newWhProduct);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return result;
        }
    }
}
