using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class DeliveryMethod
{
    public string DeliveryMedId { get; set; } = null!;

    public string DeliveryName { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
