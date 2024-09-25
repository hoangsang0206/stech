using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ReviewDislike
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime LikeDate { get; set; }

    [JsonIgnore]
    public virtual Review Review { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
