using STech.Data.Models;

namespace STech.Data.ViewModels;

public class WarehouseFilterVM
{
    public IEnumerable<Warehouse> Warehouses { get; set; } = new List<Warehouse>();

    public IEnumerable<Supplier> Suppliers { get; set; }  = new List<Supplier>();

    public string? ProductId { get; set; } = null!;

    public string? EmployeeId { get; set; }
    
    public string? SupplierId { get; set; }
    
    public string? WarehouseId { get; set; }
    
    public string? ItemId { get; set; }
    
    public string? DateRange { get; set; }
}