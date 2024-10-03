using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Review
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int Rating { get; set; }

    public DateTime CreateAt { get; set; }

    public int TotalLike { get; set; }

    public string? ReviewerName { get; set; }

    public string? ReviewerEmail { get; set; }

    public string? ReviewerPhone { get; set; }

    public string? UserId { get; set; }

    public string ProductId { get; set; } = null!;

    public bool? IsPurchased { get; set; }

    public bool? IsProceeded { get; set; }

    public bool? IsLiked { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

    public virtual ICollection<ReviewReply> ReviewReplies { get; set; } = new List<ReviewReply>();

    public virtual ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

    public virtual User? User { get; set; }
}
