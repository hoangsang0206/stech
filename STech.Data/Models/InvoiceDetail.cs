using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class InvoiceDetail
{
    public string InvoiceId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public decimal Cost { get; set; }

    public int Quantity { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
