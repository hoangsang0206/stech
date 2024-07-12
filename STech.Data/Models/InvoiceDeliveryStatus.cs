﻿using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class InvoiceDeliveryStatus
{
    public int Id { get; set; }

    public string InvoiceId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? DateUpdated { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}