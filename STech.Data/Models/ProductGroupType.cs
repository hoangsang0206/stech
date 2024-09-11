using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductGroupType
{
    public string TypeId { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<ProductGroup> ProductGroups { get; set; } = new List<ProductGroup>();
}
