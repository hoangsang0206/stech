using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Slider
{
    public int Id { get; set; }

    public string SliderImgSrc { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;
}
