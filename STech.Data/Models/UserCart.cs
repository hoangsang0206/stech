﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class UserCart
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
