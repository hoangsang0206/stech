using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class SubHeader
{
    public int Id { get; set; }

    public string? Icon { get; set; }

    public string Title { get; set; } = null!;

    public string? RedirectUrl { get; set; }
}
