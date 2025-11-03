using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class WarehouseImport
{
    public string Wiid { get; set; } = null!;

    public DateTime DateCreate { get; set; }

    public DateTime? DateImport { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public string? EmployeeId { get; set; }

    public string WarehouseId { get; set; } = null!;

    public string SupplierId { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual ICollection<WarehouseImportDetail> WarehouseImportDetails { get; set; } = new List<WarehouseImportDetail>();
}
