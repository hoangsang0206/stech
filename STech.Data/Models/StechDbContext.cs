using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace STech.Data.Models;

public partial class StechDbContext : DbContext
{
    public StechDbContext()
    {
    }

    public StechDbContext(DbContextOptions<StechDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<BannerType> BannerTypes { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; }

    public virtual DbSet<DeliveryUnit> DeliveryUnits { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Function> Functions { get; set; }

    public virtual DbSet<FunctionAuthorization> FunctionAuthorizations { get; set; }

    public virtual DbSet<FunctionCategory> FunctionCategories { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<InvoiceStatus> InvoiceStatuses { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<MenuLevel1> MenuLevel1s { get; set; }

    public virtual DbSet<MenuLevel2> MenuLevel2s { get; set; }

    public virtual DbSet<PackingSlip> PackingSlips { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductGroup> ProductGroups { get; set; }

    public virtual DbSet<ProductGroupItem> ProductGroupItems { get; set; }

    public virtual DbSet<ProductGroupType> ProductGroupTypes { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductSpecification> ProductSpecifications { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewImage> ReviewImages { get; set; }

    public virtual DbSet<ReviewLike> ReviewLikes { get; set; }

    public virtual DbSet<ReviewReply> ReviewReplies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleProduct> SaleProducts { get; set; }

    public virtual DbSet<Slider> Sliders { get; set; }

    public virtual DbSet<SpecFilterByCategory> SpecFilterByCategories { get; set; }

    public virtual DbSet<SpecFilterValue> SpecFilterValues { get; set; }

    public virtual DbSet<SubHeader> SubHeaders { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserCart> UserCarts { get; set; }

    public virtual DbSet<UserCode> UserCodes { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<WarehouseExport> WarehouseExports { get; set; }

    public virtual DbSet<WarehouseExportDetail> WarehouseExportDetails { get; set; }

    public virtual DbSet<WarehouseProduct> WarehouseProducts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=AORUS-Laptop;Database=STechDB;User Id=sang;Password=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC07B40A8AA8");

            entity.HasOne(d => d.BannerTypeNavigation).WithMany(p => p.Banners)
                .HasForeignKey(d => d.BannerType)
                .HasConstraintName("FK_Banner_Type");
        });

        modelBuilder.Entity<BannerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BannerTy__3214EC07DA92B5FB");

            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F05E165DCEC4");

            entity.Property(e => e.BrandId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.BrandName).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0B9103C118");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8242A919A");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CustomerName).HasMaxLength(50);
            entity.Property(e => e.District).HasMaxLength(30);
            entity.Property(e => e.DistrictCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Province).HasMaxLength(30);
            entity.Property(e => e.ProvinceCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Ward).HasMaxLength(50);
            entity.Property(e => e.WardCode)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DeliveryMethod>(entity =>
        {
            entity.HasKey(e => e.DeliveryMedId).HasName("PK__Delivery__C9AFB121B1B137A8");

            entity.Property(e => e.DeliveryMedId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DeliveryName).HasMaxLength(50);
        });

        modelBuilder.Entity<DeliveryUnit>(entity =>
        {
            entity.HasKey(e => e.Duid).HasName("PK__Delivery__2A5FEA4A556D0705");

            entity.Property(e => e.Duid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DUId");
            entity.Property(e => e.Duname)
                .HasMaxLength(100)
                .HasColumnName("DUName");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F1166399E49");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.CitizenId)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.District).HasMaxLength(30);
            entity.Property(e => e.DistrictCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Province).HasMaxLength(30);
            entity.Property(e => e.ProvinceCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Ward).HasMaxLength(50);
            entity.Property(e => e.WardCode)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Function>(entity =>
        {
            entity.HasKey(e => e.FuncId).HasName("PK__Function__834DE213D6905D6E");

            entity.Property(e => e.FuncId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FuncCateId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FuncName).HasMaxLength(100);

            entity.HasOne(d => d.FuncCate).WithMany(p => p.Functions)
                .HasForeignKey(d => d.FuncCateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Function_FuncCategory");
        });

        modelBuilder.Entity<FunctionAuthorization>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.FuncId }).HasName("PK__Function__A2CE103BDD323FDC");

            entity.ToTable("FunctionAuthorization");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FuncId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Func).WithMany(p => p.FunctionAuthorizations)
                .HasForeignKey(d => d.FuncId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuncAuth_Func");

            entity.HasOne(d => d.Role).WithMany(p => p.FunctionAuthorizations)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuncAuth_Role");
        });

        modelBuilder.Entity<FunctionCategory>(entity =>
        {
            entity.HasKey(e => e.FuncCateId).HasName("PK__Function__91338DED631CF284");

            entity.Property(e => e.FuncCateId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FuncCateName).HasMaxLength(100);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAB59C32660C");

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AcceptedDate).HasColumnType("datetime");
            entity.Property(e => e.CancelledDate).HasColumnType("datetime");
            entity.Property(e => e.CompletedDate).HasColumnType("datetime");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DeliveryAddress).HasMaxLength(200);
            entity.Property(e => e.DeliveryMedId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMedId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus).HasMaxLength(30);
            entity.Property(e => e.RecipientName).HasMaxLength(50);
            entity.Property(e => e.RecipientPhone)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Total).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Invoice_Customer");

            entity.HasOne(d => d.DeliveryMed).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.DeliveryMedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Delivery");

            entity.HasOne(d => d.Employee).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Invoice_Employee");

            entity.HasOne(d => d.PaymentMed).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PaymentMedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_PaymentMethod");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Invoice_User");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ProductId }).HasName("PK__InvoiceD__1CD666D903179831");

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SaleId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Detail_Invoice");

            entity.HasOne(d => d.Product).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoice_Product");

            entity.HasOne(d => d.Sale).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.SaleId)
                .HasConstraintName("FK_InvoiceProduct_Sale");
        });

        modelBuilder.Entity<InvoiceStatus>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.InvoiceId }).HasName("PK__InvoiceS__7F6D86AC0F55D53F");

            entity.ToTable("InvoiceStatus");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(100);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceStatuses)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Status_Invoice");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menu__3214EC078BB706C6");

            entity.ToTable("Menu");

            entity.Property(e => e.MenuIcon).HasMaxLength(100);
            entity.Property(e => e.MenuName).HasMaxLength(50);
        });

        modelBuilder.Entity<MenuLevel1>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MenuLeve__3214EC07490F7D0A");

            entity.ToTable("MenuLevel1");

            entity.Property(e => e.MenuName).HasMaxLength(50);

            entity.HasOne(d => d.Menu).WithMany(p => p.MenuLevel1s)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menu1_Menu");
        });

        modelBuilder.Entity<MenuLevel2>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__MenuLeve__3214EC0755BF88C8");

            entity.ToTable("MenuLevel2");

            entity.Property(e => e.MenuName).HasMaxLength(50);

            entity.HasOne(d => d.MenuLevel1).WithMany(p => p.MenuLevel2s)
                .HasForeignKey(d => d.MenuLevel1Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menu2_Menu1");
        });

        modelBuilder.Entity<PackingSlip>(entity =>
        {
            entity.HasKey(e => e.Psid).HasName("PK__PackingS__BC000956631FED4B");

            entity.HasIndex(e => e.InvoiceId, "UQ__PackingS__D796AAB4A2975625").IsUnique();

            entity.Property(e => e.Psid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PSId");
            entity.Property(e => e.DeliveryFee).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.DeliveryUnitId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsCompleted).HasDefaultValue(false);

            entity.HasOne(d => d.DeliveryUnit).WithMany(p => p.PackingSlips)
                .HasForeignKey(d => d.DeliveryUnitId)
                .HasConstraintName("FK_PackingSlip_DeliveryUnit");

            entity.HasOne(d => d.Employee).WithMany(p => p.PackingSlips)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_PackingSlip_Employee");

            entity.HasOne(d => d.Invoice).WithOne(p => p.PackingSlip)
                .HasForeignKey<PackingSlip>(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PackingSlip_Invoice");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMedId).HasName("PK__PaymentM__1D98A1A732B5DBE6");

            entity.Property(e => e.PaymentMedId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LogoSrc).IsUnicode(false);
            entity.Property(e => e.PaymentName).HasMaxLength(50);
            entity.Property(e => e.Sort).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDF6793729");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BrandId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.DateDeleted).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductName).HasMaxLength(200);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_SP_HSX");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_SP_DM");
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductG__3214EC0706C83E36");

            entity.Property(e => e.BackgroundColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("#ffffff");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);
            entity.Property(e => e.GroupTypeId)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.HeaderTextColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("#000000");

            entity.HasOne(d => d.GroupType).WithMany(p => p.ProductGroups)
                .HasForeignKey(d => d.GroupTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PGroup_PGroupType");
        });

        modelBuilder.Entity<ProductGroupItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductG__3214EC07EBE2DBB4");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Group).WithMany(p => p.ProductGroupItems)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PGroupItem_Group");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductGroupItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PGroupItem_Product");
        });

        modelBuilder.Entity<ProductGroupType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__ProductG__516F03B59FC4CF41");

            entity.Property(e => e.TypeId)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC07AFDDDCF3");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Img_Product");
        });

        modelBuilder.Entity<ProductSpecification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductS__3214EC073B99A0E4");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SpecName).HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductSpecifications)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Spec_Product");

            entity.HasOne(d => d.SpecFilter).WithMany(p => p.ProductSpecifications)
                .HasForeignKey(d => d.SpecFilterId)
                .HasConstraintName("FK_Spec_SpecFilter");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC0763D41091");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.CreateAt).HasColumnType("datetime");
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReviewerEmail).HasMaxLength(255);
            entity.Property(e => e.ReviewerName).HasMaxLength(50);
            entity.Property(e => e.ReviewerPhone)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Product");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Review_User");
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReviewIm__3214EC0717D64B44");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RVImages_Review");
        });

        modelBuilder.Entity<ReviewLike>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReviewLi__3214EC0799937885");

            entity.Property(e => e.LikeDate).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewLikes)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewLike_Review");

            entity.HasOne(d => d.User).WithMany(p => p.ReviewLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReviewLike_User");
        });

        modelBuilder.Entity<ReviewReply>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReviewRe__3214EC07385DB39A");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.ReplyDate).HasColumnType("datetime");
            entity.Property(e => e.UserReplyId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewReplies)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RVReplies_Review");

            entity.HasOne(d => d.UserReply).WithMany(p => p.ReviewReplies)
                .HasForeignKey(d => d.UserReplyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RVRepiles_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1ACFDA3797");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("PK__Sales__1EE3C3FF724B7144");

            entity.Property(e => e.SaleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BackgroundColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("#ffffff");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.HeaderTextColor)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("#000000");
            entity.Property(e => e.SaleName).HasMaxLength(100);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<SaleProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SaleProd__3214EC0738C48EC2");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SaleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleProduct_Product");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleProducts)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleProduct_Sale");
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sliders__3214EC07B44944C4");
        });

        modelBuilder.Entity<SpecFilterByCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpecFilt__3214EC078351989F");

            entity.ToTable("SpecFilterByCategory");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SpecFilterType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.SpecFilterTypeDisplay).HasMaxLength(30);
        });

        modelBuilder.Entity<SpecFilterValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SpecFilt__3214EC07A38D3CF4");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.SpecFilterValue1)
                .HasMaxLength(50)
                .HasColumnName("SpecFilterValue");
            entity.Property(e => e.SpecFilterValueDisplay).HasMaxLength(50);

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.SpecFilterValue)
                .HasForeignKey<SpecFilterValue>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SpecFilterValue_SpecFilter");
        });

        modelBuilder.Entity<SubHeader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SubHeade__3214EC0705BD1B6C");

            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666B4D9BBA1C7");

            entity.Property(e => e.SupplierId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SupplierName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDFB78CC7");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AuthenticationProvider)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmailConfirmed).HasDefaultValue(false);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.PasswordHash).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PhoneConfirmed).HasDefaultValue(false);
            entity.Property(e => e.RandomKey).HasMaxLength(50);
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_USER_EMPLOYEE");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.UserId }).HasName("PK__UserAddr__E36C60C39E6EE1B7");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.AddressType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.District).HasMaxLength(30);
            entity.Property(e => e.DistrictCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.Province).HasMaxLength(30);
            entity.Property(e => e.ProvinceCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.RecipientName).HasMaxLength(50);
            entity.Property(e => e.RecipientPhone)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Ward).HasMaxLength(50);
            entity.Property(e => e.WardCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Address_User");
        });

        modelBuilder.Entity<UserCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserCart__3214EC0730C44DBC");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.UserCarts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_Product");

            entity.HasOne(d => d.User).WithMany(p => p.UserCarts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<UserCode>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CodeType)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.CodeValue).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsExpired).HasDefaultValue(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Code_User");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__2608AFF9F444B510");

            entity.Property(e => e.WarehouseId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.District).HasMaxLength(30);
            entity.Property(e => e.DistrictCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.Longtitude).HasColumnType("decimal(18, 10)");
            entity.Property(e => e.Province).HasMaxLength(30);
            entity.Property(e => e.ProvinceCode)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Ward).HasMaxLength(50);
            entity.Property(e => e.WardCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.WarehouseName).HasMaxLength(100);
        });

        modelBuilder.Entity<WarehouseExport>(entity =>
        {
            entity.HasKey(e => e.Weid).HasName("PK__Warehous__FA31005192D7BD96");

            entity.Property(e => e.Weid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WEId");
            entity.Property(e => e.DateCreate).HasColumnType("datetime");
            entity.Property(e => e.DateExport).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.ReasonExport).HasMaxLength(100);
            entity.Property(e => e.WarehouseId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.WarehouseExports)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_WE_Employee");

            entity.HasOne(d => d.Invoice).WithMany(p => p.WarehouseExports)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_WE_Invoice");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseExports)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WE_Warehouse");
        });

        modelBuilder.Entity<WarehouseExportDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Warehous__3214EC07F8FCAAC9");

            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Weid)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WEId");

            entity.HasOne(d => d.Product).WithMany(p => p.WarehouseExportDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WEDetail_Product");

            entity.HasOne(d => d.We).WithMany(p => p.WarehouseExportDetails)
                .HasForeignKey(d => d.Weid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WHDetail_WHE");
        });

        modelBuilder.Entity<WarehouseProduct>(entity =>
        {
            entity.HasKey(e => new { e.WarehouseId, e.ProductId }).HasName("PK__Warehous__ED486395FBE0EFEB");

            entity.Property(e => e.WarehouseId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.WarehouseProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WP_Product");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.WarehouseProducts)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WP_kho");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
