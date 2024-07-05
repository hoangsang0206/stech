using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class DeliveryUnit
{
    public string Duid { get; set; } = null!;

    public string Duname { get; set; } = null!;

    public string? Phone { get; set; }

    public string? LogoSrc { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
