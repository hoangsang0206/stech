using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ProductGroup
{
    public int Id { get; set; }

    public string GroupTypeId { get; set; } = null!;

    public string GroupName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string? BackgroundColor { get; set; }

    public string? HeaderTextColor { get; set; }

    public bool IsActive { get; set; }

    [JsonIgnore]
    public virtual ProductGroupType GroupType { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<ProductGroupItem> ProductGroupItems { get; set; } = new List<ProductGroupItem>();
}
