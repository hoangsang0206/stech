using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class WarehouseImportHistory
{
    public string HistoryId { get; set; } = null!;

    public string Wiid { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public DateTime ImportDate { get; set; }

    public string BatchNumber { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual WarehouseImport Wi { get; set; } = null!;
}
