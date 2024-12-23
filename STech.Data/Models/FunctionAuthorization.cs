﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class FunctionAuthorization
{
    public int GroupId { get; set; }

    public string FuncId { get; set; } = null!;

    public bool IsAuthorized { get; set; }

    [JsonIgnore]
    public virtual Function Func { get; set; } = null!;

    public virtual UserGroup Group { get; set; } = null!;
}
