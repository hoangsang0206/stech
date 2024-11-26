using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace STech.Data.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    public int? ManufacturedYear { get; set; }

    public decimal? OriginalPrice { get; set; }

    public decimal Price { get; set; }

    public int? Warranty { get; set; }

    public string? CategoryId { get; set; }

    public string? BrandId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DateAdded { get; set; }

    public DateTime? DateDeleted { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    [JsonIgnore]
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    [JsonIgnore]
    public virtual ICollection<ProductGroupItem> ProductGroupItems { get; set; } = new List<ProductGroupItem>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductSpecification> ProductSpecifications { get; set; } = new List<ProductSpecification>();

    [JsonIgnore]
    public virtual ICollection<ReturnExchangeSlip> ReturnExchangeSlips { get; set; } = new List<ReturnExchangeSlip>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();

    [JsonIgnore]
    public virtual ICollection<UserCart> UserCarts { get; set; } = new List<UserCart>();

    [JsonIgnore]
    public virtual ICollection<WarehouseExportDetail> WarehouseExportDetails { get; set; } = new List<WarehouseExportDetail>();

    [JsonIgnore]
    public virtual ICollection<WarehouseImportDetail> WarehouseImportDetails { get; set; } = new List<WarehouseImportDetail>();

    [JsonIgnore]
    public virtual ICollection<WarehouseImportHistory> WarehouseImportHistories { get; set; } = new List<WarehouseImportHistory>();

    public virtual ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();

    [JsonIgnore]
    public virtual ICollection<WarrantySlip> WarrantySlips { get; set; } = new List<WarrantySlip>();
}
