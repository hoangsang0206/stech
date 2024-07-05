using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class MenuLevel1
{
    public int Id { get; set; }

    public string MenuName { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public int MenuId { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual ICollection<MenuLevel2> MenuLevel2s { get; set; } = new List<MenuLevel2>();
}
