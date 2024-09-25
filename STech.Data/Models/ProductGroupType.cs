using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ProductGroupType
{
    public string TypeId { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<ProductGroup> ProductGroups { get; set; } = new List<ProductGroup>();
}
