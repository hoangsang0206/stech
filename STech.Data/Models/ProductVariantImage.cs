using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductVariantImage
{
    public int Id { get; set; }

    public int VariantId { get; set; }

    public string ImageSrc { get; set; } = null!;

    public virtual ProductVariant Variant { get; set; } = null!;
}
