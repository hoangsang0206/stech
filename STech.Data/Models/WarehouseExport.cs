using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class WarehouseExport
{
    public string Weid { get; set; } = null!;

    public DateTime DateCreate { get; set; }

    public DateTime? DateExport { get; set; }

    public string? ReasonExport { get; set; }

    public string? Note { get; set; }

    public string? InvoiceId { get; set; }

    public string? EmployeeId { get; set; }

    public string WarehouseId { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
