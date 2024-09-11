﻿using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class SaleProduct
{
    public int Id { get; set; }

    public string SaleId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public decimal SalePrice { get; set; }

    public int SaleQuantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;
}
