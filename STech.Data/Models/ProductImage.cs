using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public string ImageSrc { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
