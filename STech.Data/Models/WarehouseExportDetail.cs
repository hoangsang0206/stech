using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseExportDetail
{
    public int Id { get; set; }

    public string Weid { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int RequestedQuantity { get; set; }

    public int ActualQuantity { get; set; }

    public decimal UnitPrice { get; set; }
    
    public virtual Product Product { get; set; } = null!;

    [JsonIgnore]
    public virtual WarehouseExport We { get; set; } = null!;
}
