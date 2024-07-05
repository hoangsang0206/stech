using System;
using System.Collections.Generic;

namespace STech.Data.Models;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateOnly? Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string CitizenId { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
