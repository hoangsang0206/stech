using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class PaymentMethod
{
    public string PaymentMedId { get; set; } = null!;

    public string PaymentName { get; set; } = null!;

    public string? LogoSrc { get; set; }

    public bool? IsActive { get; set; }

    public int? Sort { get; set; }

    [JsonIgnore]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
