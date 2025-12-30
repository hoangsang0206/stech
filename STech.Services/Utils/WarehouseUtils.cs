using STech.Data.Models;

namespace STech.Services.Utils;

public static class WarehouseUtils
{
    public static IQueryable<WarehouseImport> FilterByWarehouse(this IQueryable<WarehouseImport> query, string warehouseId)
    {
        return query.Where(w => w.WarehouseId == warehouseId);
    }
    
    public static IQueryable<WarehouseImport> FilterBySupplier(this IQueryable<WarehouseImport> query, string supplierId)
    {
        return query.Where(w => w.SupplierId == supplierId);
    }
    
    public static IQueryable<WarehouseImport> FilterByProduct(this IQueryable<WarehouseImport> query, string productId)
    {
        return query.Where(w => w.WarehouseImportDetails.Any(t => t.ProductId == productId));
    }
    
    public static IQueryable<WarehouseImport> FilterByEmployee(this IQueryable<WarehouseImport> query, string employeeId)
    {
        return query.Where(w => w.EmployeeId == employeeId);
    }
    
    public static IQueryable<WarehouseImport> FilterByDate(this IQueryable<WarehouseImport> query, DateTime from, DateTime to)
    {
        return query.Where(t => t.DateCreate.Date >= from.Date && t.DateCreate.Date <= to.Date);
    }

    public static IQueryable<WarehouseImport> Sort(this IQueryable<WarehouseImport> query, string sort)
    {
        
        
        return query;
    }
}