using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class DeliveryMethod
{
    public string DeliveryMedId { get; set; } = null!;

    public string DeliveryName { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
