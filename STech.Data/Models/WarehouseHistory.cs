using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class WarehouseHistory
{
    public string HistoryId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public string WarehouseId { get; set; } = null!;

    public string? BatchNumber { get; set; }

    public string ActionType { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? ReferenceId { get; set; }

    public DateTime? ActionDate { get; set; }
    
    public virtual Product Product { get; set; } = null!;
    
    public virtual Warehouse Warehouse { get; set; } = null!;
}
