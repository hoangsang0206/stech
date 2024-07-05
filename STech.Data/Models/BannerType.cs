using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class BannerType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();
}
