﻿using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Invoice
{
    public string InvoiceId { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Total { get; set; }

    public string PaymentMedId { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string DeliveryMedId { get; set; } = null!;

    public string? DeliveryAddress { get; set; }

    public string RecipientPhone { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string? Note { get; set; }

    public bool? IsCompleted { get; set; }

    public string? CustomerId { get; set; }

    public string? UserId { get; set; }

    public string? EmployeeId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual DeliveryMethod DeliveryMed { get; set; } = null!;

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<InvoiceStatus> InvoiceStatuses { get; set; } = new List<InvoiceStatus>();

    public virtual PackingSlip? PackingSlip { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<WarehouseExport> WarehouseExports { get; set; } = new List<WarehouseExport>();
}
