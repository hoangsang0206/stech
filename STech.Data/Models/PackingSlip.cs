using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class PackingSlip
{
    public string Psid { get; set; } = null!;

    public string InvoiceId { get; set; } = null!;

    public string? DeliveryUnitId { get; set; }

    public string? EmployeeId { get; set; }

    public bool? IsCompleted { get; set; }

    public decimal DeliveryFee { get; set; }

    public virtual DeliveryUnit? DeliveryUnit { get; set; }

    public virtual Employee? Employee { get; set; }

    [JsonIgnore]
    public virtual Invoice Invoice { get; set; } = null!;
}
