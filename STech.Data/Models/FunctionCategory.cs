using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class FunctionCategory
{
    public string FuncCateId { get; set; } = null!;

    public string FuncCateName { get; set; } = null!;

    public string? IconSrc { get; set; }

    [JsonIgnore]
    public virtual ICollection<Function> Functions { get; set; } = new List<Function>();
}
