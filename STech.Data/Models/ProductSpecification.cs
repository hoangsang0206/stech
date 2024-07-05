using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductSpecification
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public string SpecName { get; set; } = null!;

    public string SpecValue { get; set; } = null!;

    public int? SpecFilterId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual SpecFilterValue? SpecFilter { get; set; }
}
