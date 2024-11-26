using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ReviewReply
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public string? Content { get; set; }

    public DateTime ReplyDate { get; set; }

    public string UserReplyId { get; set; } = null!;

    public bool? IsRead { get; set; }

    [JsonIgnore]
    public virtual Review Review { get; set; } = null!;

    public virtual User UserReply { get; set; } = null!;
}
