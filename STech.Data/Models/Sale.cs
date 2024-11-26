using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Sale
{
    public string SaleId { get; set; } = null!;

    public string SaleName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? BackgroundColor { get; set; }

    public string? HeaderTextColor { get; set; }

    public bool? IsActive { get; set; }

    [JsonIgnore]
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    
    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();
}
