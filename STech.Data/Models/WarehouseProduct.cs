using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class WarehouseProduct
{
    public string WarehouseId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
