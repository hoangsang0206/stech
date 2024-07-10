using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class PackingSlipStatus
{
    public int Id { get; set; }

    public string Psid { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? DateUpdated { get; set; }

    public virtual PackingSlip Ps { get; set; } = null!;
}
