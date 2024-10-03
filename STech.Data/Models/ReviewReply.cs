using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class ReviewReply
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime ReplyDate { get; set; }

    public bool? IsRead { get; set; }

    public string UserReplyId { get; set; } = null!;

    [JsonIgnore]
    public virtual Review Review { get; set; } = null!;

    public virtual User UserReply { get; set; } = null!;
}
