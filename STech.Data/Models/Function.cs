using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Function
{
    public string FuncId { get; set; } = null!;

    public string FuncName { get; set; } = null!;

    public string FuncCateId { get; set; } = null!;

    public string? IconSrc { get; set; }

    [JsonIgnore]
    public virtual FunctionCategory FuncCate { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<FunctionAuthorization> FunctionAuthorizations { get; set; } = new List<FunctionAuthorization>();
}
