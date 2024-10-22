using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ReturnExchangeSlip
{
    public string Resid { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public DateTime Resdate { get; set; }

    public string Reason { get; set; } = null!;

    public string? Status { get; set; }

    public string? Note { get; set; }

    public string ProductId { get; set; } = null!;

    public string InvoiceId { get; set; } = null!;

    public string EmployeeId { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
