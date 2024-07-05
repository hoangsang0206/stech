using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductVariant
{
    public int VariantId { get; set; }

    public int AttrId { get; set; }

    public string AttrValue { get; set; } = null!;

    public decimal ExtraPrice { get; set; }

    public string ProductId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductVariantImage> ProductVariantImages { get; set; } = new List<ProductVariantImage>();
}
