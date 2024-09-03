using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class ReviewImage
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public virtual Review Review { get; set; } = null!;
}
