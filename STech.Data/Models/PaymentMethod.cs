using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class PaymentMethod
{
    public string PaymentMedId { get; set; } = null!;

    public string PaymentName { get; set; } = null!;

    public string? LogoSrc { get; set; }

    public bool? IsActive { get; set; }

    public int Sort { get; set; }
}
