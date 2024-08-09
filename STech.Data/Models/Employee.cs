using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

    [JsonIgnore]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [JsonIgnore]
    public virtual ICollection<PackingSlip> PackingSlips { get; set; } = new List<PackingSlip>();

    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [JsonIgnore]
    public virtual ICollection<WarehouseExport> WarehouseExports { get; set; } = new List<WarehouseExport>();
}
