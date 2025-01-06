using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ProductSpecification
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public string SpecName { get; set; } = null!;

    public string SpecValue { get; set; } = null!;

    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
}
