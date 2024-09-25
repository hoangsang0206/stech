﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    [JsonIgnore]
    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string? Phone { get; set; }

    public bool? PhoneConfirmed { get; set; }

    public string? Avatar { get; set; }

    [JsonIgnore]
    public string RandomKey { get; set; } = null!;

    public string? FullName { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public bool? IsActive { get; set; }

    public string RoleId { get; set; } = null!;

    public string? AuthenticationProvider { get; set; }

    public string? EmployeeId { get; set; }

    [JsonIgnore]
    public virtual Employee? Employee { get; set; }

    [JsonIgnore]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Role Role { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    [JsonIgnore]
    public virtual ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();

    [JsonIgnore]
    public virtual ICollection<ReviewReply> ReviewReplies { get; set; } = new List<ReviewReply>();

    [JsonIgnore]
    public virtual ICollection<ReviewDislike> ReviewDislikes { get; set; } = new List<ReviewDislike>();

    [JsonIgnore]
    public virtual ICollection<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();

    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
