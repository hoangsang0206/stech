using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ProductGroupItem
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public string ProductId { get; set; } = null!;

    [JsonIgnore]
    public virtual ProductGroup Group { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
