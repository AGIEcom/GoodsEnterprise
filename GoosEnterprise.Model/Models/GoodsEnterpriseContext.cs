using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GoodsEnterprise.Model.Models
{
    public partial class GoodsEnterpriseContext : DbContext
    {
        public GoodsEnterpriseContext()
        {
        }

        public GoodsEnterpriseContext(DbContextOptions<GoodsEnterpriseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<BaseCost> BaseCosts { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerBasket> CustomerBaskets { get; set; }
        public virtual DbSet<CustomerFavourite> CustomerFavourites { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<PromotionCost> PromotionCosts { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Status> Statuses { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<ProductList> ProductLists { get; set; }
        public virtual DbSet<PromotionCostList> PromotionCostLists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=GoodsEnterprise;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admin");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Admins)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Admin__RoleId__59063A47");
            });

            modelBuilder.Entity<BaseCost>(entity =>
            {
                entity.ToTable("BaseCost");

                entity.Property(e => e.BaseCostId).HasColumnName("BaseCostID");

                entity.Property(e => e.BaseCost1)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("BaseCost");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.BaseCosts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_BaseCost_Product");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.BaseCosts)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_BaseCost_Supplier");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl200)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ImageUrl_200");

                entity.Property(e => e.ImageUrl500)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ImageUrl_500");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl200)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ImageUrl_200");

                entity.Property(e => e.ImageUrl500)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ImageUrl_500");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.Address1)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyEmail)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyFax)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CompanyPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.County)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.HomePhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.PasswordExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Customers)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Customer__RoleId__59FA5E80");
            });

            modelBuilder.Entity<CustomerBasket>(entity =>
            {
                entity.ToTable("CustomerBasket");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerBaskets)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerB__Custo__5AEE82B9");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CustomerBaskets)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerB__Produ__52593CB8");
            });

            modelBuilder.Entity<CustomerFavourite>(entity =>
            {
                entity.ToTable("CustomerFavourite");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerFavourites)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerF__Custo__5CD6CB2B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CustomerFavourites)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CustomerF__Produ__5629CD9C");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.Cases).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Cbm)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("CBM");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DatePlaced).HasColumnType("datetime");

                entity.Property(e => e.Gbp)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("GBP");

                entity.Property(e => e.Kg)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("KG");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__CustomerI__59FA5E80");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__Status");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(16, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__5CD6CB2B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__5DCAEF64");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.CaseDepthMm).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.CaseHeightMm).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.CasePrice).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.CaseWidthMm).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpriyDate).HasColumnType("datetime");

                entity.Property(e => e.GrossCaseWeightKg).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Image)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.InnerEan)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("InnerEAN");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NetCaseWeightKg).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.OuterEan)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("OuterEAN");

                entity.Property(e => e.PackDepth).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PackHeight).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PackWidth).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PalletDepthMeter).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PalletHeightMeter).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PalletWeightKg).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PalletWidthMeter).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.ProductDescription)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.Seebelow)
                    .HasMaxLength(500)
                    .IsUnicode(false);
                entity.Property(e => e.Seebelow1)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProductName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TaxslabId).HasColumnName("TaxslabID");

                entity.Property(e => e.UnitSize)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Upc).HasColumnName("UPC");

                entity.HasOne(d => d.Taxslab)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.TaxslabId)
                    .HasConstraintName("FK_Product_Tax");
            });

            modelBuilder.Entity<PromotionCost>(entity =>
            {
                entity.ToTable("PromotionCost");

                entity.Property(e => e.PromotionCostId).HasColumnName("PromotionCostID");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.PromotionCost1)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("PromotionCost");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.SupplierId).HasColumnName("SupplierID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.PromotionCosts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_PromotionCost_PromotionCost");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.PromotionCosts)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK_PromotionCost_Supplier");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("Status");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.ToTable("SubCategory");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.Property(e => e.Address1)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Address2)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Skucode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SKUCode");
            });

            modelBuilder.Entity<Tax>(entity =>
            {
                entity.ToTable("Tax");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Value).HasColumnType("decimal(16, 2)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
