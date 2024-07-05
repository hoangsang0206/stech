using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string? Phone { get; set; }

    public bool? PhoneConfirmed { get; set; }

    public string? Avatar { get; set; }

    public string RandomKey { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string RoleId { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public virtual ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();
}
