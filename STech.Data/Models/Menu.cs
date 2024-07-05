using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Menu
{
    public int Id { get; set; }

    public string MenuName { get; set; } = null!;

    public string RedirectUrl { get; set; } = null!;

    public string MenuIcon { get; set; } = null!;

    public virtual ICollection<MenuLevel1> MenuLevel1s { get; set; } = new List<MenuLevel1>();
}
