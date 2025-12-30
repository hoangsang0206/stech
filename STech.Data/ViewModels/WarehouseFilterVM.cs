using STech.Data.Models;

namespace STech.Data.ViewModels;

public class WarehouseFilterVM
{
    public IEnumerable<Warehouse> Warehouses { get; set; }

    public IEnumerable<Supplier> Suppliers { get; set; }

    public string ProductId { get; set; }

    public string EmployeeId { get; set; }
}