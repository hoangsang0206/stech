using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseImportDetail
{
    public int Id { get; set; }

    public string Wiid { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string? BatchNumber { get; set; }

    public virtual Product Product { get; set; } = null!;

    [JsonIgnore]
    public virtual WarehouseImport Wi { get; set; } = null!;
}
