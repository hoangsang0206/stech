using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseProduct
{
    public string WarehouseId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;

    [JsonIgnore]
    public virtual Warehouse Warehouse { get; set; } = null!;
}
