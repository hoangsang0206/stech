using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Banner
{
    public int Id { get; set; }

    public string BannerImgSrc { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public int? BannerType { get; set; }

    public virtual BannerType? BannerTypeNavigation { get; set; }
}
