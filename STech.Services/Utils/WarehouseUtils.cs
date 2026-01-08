using STech.Data.Models;

namespace STech.Services.Utils;

public static class WarehouseUtils
{
    const string DateCreateASC = "date-create-asc";
    const string DateCreateDESC = "date-create-desc";
    const string DateImportASC = "date-import-asc";
    const string DateImportDESC = "date-import-desc";
    const string TotalPriceASC = "total-price-asc";
    const string TotalPriceDESC = "total-price-desc";
    const string TotalQtyASC = "total-qty-asc";
    const string TotalQtyDESC = "total-qty-desc";
    
    public static IQueryable<WarehouseImport> FilterByWarehouse(this IQueryable<WarehouseImport> query, string? warehouseId)
    {
        return string.IsNullOrEmpty(warehouseId) ? query : query.Where(w => w.WarehouseId == warehouseId);
    }
    
    public static IQueryable<WarehouseImport> FilterBySupplier(this IQueryable<WarehouseImport> query, string? supplierId)
    {
        return string.IsNullOrEmpty(supplierId) ? query : query.Where(w => w.SupplierId == supplierId);
    }
    
    public static IQueryable<WarehouseImport> FilterByProduct(this IQueryable<WarehouseImport> query, string? productId)
    {
        return string.IsNullOrEmpty(productId) ? query : query.Where(w => w.WarehouseImportDetails.Any(t => t.ProductId == productId));
    }
    
    public static IQueryable<WarehouseImport> FilterByEmployee(this IQueryable<WarehouseImport> query, string? employeeId)
    {
        return string.IsNullOrEmpty(employeeId) ? query : query.Where(w => w.EmployeeId == employeeId);
    }
    
    public static IQueryable<WarehouseImport> FilterByDateCreate(this IQueryable<WarehouseImport> query, DateTime from, DateTime to)
    {
        return query.Where(t => t.DateCreate.Date >= from.Date && t.DateCreate.Date <= to.Date);
    }
    
    public static IQueryable<WarehouseImport> FilterByDateImport(this IQueryable<WarehouseImport> query, DateTime from, DateTime to)
    {
        return query.Where(t => t.DateImport != null 
                                && (t.DateImport.Value.Date >= from.Date && t.DateImport.Value.Date <= to.Date));
    }
    
    public static IQueryable<WarehouseImport> FilterByDateImport(this IQueryable<WarehouseImport> query, string? dateRange)
    {
        if (string.IsNullOrEmpty(dateRange)) return query;

        try
        {
            string[] parts = dateRange.Split(" - ");
            DateTime from = DateTime.Parse(parts[0]);
            DateTime to = DateTime.Parse(parts[1]);

            return query.Where(t => t.DateImport != null
                                    && (t.DateImport.Value.Date >= from.Date && t.DateImport.Value.Date <= to.Date));
        }
        catch (Exception)
        {
            return query;
        }
    }

    public static IQueryable<WarehouseImport> FilterByStatus(this IQueryable<WarehouseImport> query, string? status)
    {
        return string.IsNullOrEmpty(status) ? query : query.Where(w => w.Status == status);
    }

    public static IQueryable<WarehouseImport> Sort(this IQueryable<WarehouseImport> query, string? sort)
    {
        switch (sort)
        {
            case DateCreateASC:
                return query.OrderBy(t => t.DateCreate);
            
            case DateCreateDESC:
                return query.OrderByDescending(t => t.DateCreate);
            
            case DateImportASC:
                return query.OrderBy(t => t.DateImport);
            
            case DateImportDESC:
                return query.OrderByDescending(t => t.DateImport);
            
            case TotalPriceASC:
                return query.Select(t => new
                {
                    Import = t,
                    Total = t.WarehouseImportDetails.Sum(i => i.UnitPrice * i.Quantity)
                })
                .OrderBy(t => t.Total)
                .Select(x => x.Import);
            
            case TotalPriceDESC:
                return query.Select(t => new
                    {
                        Import = t,
                        Total = t.WarehouseImportDetails.Sum(i => i.UnitPrice * i.Quantity)
                    })
                    .OrderByDescending(t => t.Total)
                    .Select(x => x.Import);
            
            case TotalQtyASC:
                return query.Select(t => new
                    {
                        Import = t,
                        Total = t.WarehouseImportDetails.Sum(i => i.Quantity)
                    })
                    .OrderBy(t => t.Total)
                    .Select(x => x.Import);
            
            case TotalQtyDESC:
                return query.Select(t => new
                    {
                        Import = t,
                        Total = t.WarehouseImportDetails.Sum(i => i.Quantity)
                    })
                    .OrderByDescending(t => t.Total)
                    .Select(x => x.Import);
        }
        
        return query;
    }
}