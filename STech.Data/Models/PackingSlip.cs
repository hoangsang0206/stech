using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class PackingSlip
{
    public string Psid { get; set; } = null!;

    public string InvoiceId { get; set; } = null!;

    public string DeliveryUnitId { get; set; } = null!;

    public string? EmployeeId { get; set; }

    public bool? IsCompleted { get; set; }

    public decimal DeliveryFee { get; set; }

    public virtual DeliveryUnit DeliveryUnit { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual ICollection<PackingSlipStatus> PackingSlipStatuses { get; set; } = new List<PackingSlipStatus>();
}
