using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class UserCode
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string CodeType { get; set; } = null!;

    public string CodeValue { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int MinutesToExpire { get; set; }

    public bool? IsExpired { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
