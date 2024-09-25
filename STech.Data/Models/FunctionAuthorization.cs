using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class FunctionAuthorization
{
    public string RoleId { get; set; } = null!;

    public string FuncId { get; set; } = null!;

    public bool IsAuthorized { get; set; }

    [JsonIgnore]
    public virtual Function Func { get; set; } = null!;

    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;
}
