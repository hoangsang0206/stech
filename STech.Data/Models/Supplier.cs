using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Supplier
{
    public string SupplierId { get; set; } = null!;

    public string SupplierName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<WarehouseImport> WarehouseImports { get; set; } = new List<WarehouseImport>();
}
