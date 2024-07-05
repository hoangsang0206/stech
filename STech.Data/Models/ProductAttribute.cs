using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductAttribute
{
    public int AttrId { get; set; }

    public string AttrName { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
