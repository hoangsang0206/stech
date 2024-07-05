using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class SpecFilterByCategory
{
    public int Id { get; set; }

    public string CategoryId { get; set; } = null!;

    public string? SpecFilterType { get; set; }

    public string? SpecFilterTypeDisplay { get; set; }

    public virtual SpecFilterValue? SpecFilterValue { get; set; }
}
