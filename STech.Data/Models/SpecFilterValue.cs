using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class SpecFilterValue
{
    public int Id { get; set; }

    public int SpecFilterId { get; set; }

    public string SpecFilterValue1 { get; set; } = null!;

    public string SpecFilterValueDisplay { get; set; } = null!;

    public virtual SpecFilterByCategory IdNavigation { get; set; } = null!;

    public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();
}
