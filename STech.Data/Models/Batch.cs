using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Batch
{
    public string BatchNumber { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string WarehouseId { get; set; } = null!;

    public int CurrentStock { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
