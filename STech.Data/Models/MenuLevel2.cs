using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class MenuLevel2
{
    public int Id { get; set; }

    public string MenuName { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public int MenuLevel1Id { get; set; }

    public virtual MenuLevel1 MenuLevel1 { get; set; } = null!;
}
