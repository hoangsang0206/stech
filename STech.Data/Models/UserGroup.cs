using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class UserGroup
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public bool? HasAllPermissions { get; set; }

    public virtual ICollection<FunctionAuthorization> FunctionAuthorizations { get; set; } = new List<FunctionAuthorization>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
