using FurnitureStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace FurnitureStoreBE.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> Tokens { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Designer> Designer { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<RoomSpace> RoomSpaces { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<FurnitureType> FurnitureTypes  { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<UserUsedCoupon> UserUsedCoupon { get; set; }   
        public DbSet<Notification> Notification { get; set; }
        public DbSet<AspNetTypeClaims> TypeClaims { get; set; }
        public DbSet<AspNetRoleClaims<string>> RoleClaims { get; set; }
        public DbSet<ImportInvoice> ImportInvoices { get; set; }
        public DbSet<ImportItem> ImportItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            // User Relationships
            modelBuilder.Entity<Asset>()
                .HasOne(p => p.User)
                .WithOne(p => p.Asset)
                .HasForeignKey<User>(p => p.AssetId);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Tokens)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Addresses)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            // Brand Relationship
            modelBuilder.Entity<Asset>()
                .HasOne(p => p.Brand)
                .WithOne(p => p.Asset)
                .HasForeignKey<Brand>(p => p.AssetId);

            modelBuilder.Entity<Brand>()
                .HasMany(p => p.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category relationship 
            modelBuilder.Entity<Asset>()
                .HasOne(p => p.Category)
                .WithOne(p => p.Asset)
                .HasForeignKey<Category>(p => p.AssetId);

            modelBuilder.Entity<Category>()
               .HasMany(p => p.Products)
               .WithOne(p => p.Category)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

            // Color relationship
            modelBuilder.Entity<Color>()
                .HasMany(p => p.ProductVariants)
                .WithOne(p => p.Color)
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Desginer Relationship
            modelBuilder.Entity<Asset>()
                .HasOne(p => p.Designer)
                .WithOne(p => p.Asset)
                .HasForeignKey<Designer>(p => p.AssetId);

            // Coupon relationship
            modelBuilder.Entity<Coupon>()
               .HasIndex(e => e.Code)
               .IsUnique();
            modelBuilder.Entity<Asset>()
               .HasOne(p => p.Coupon)
               .WithOne(p => p.Asset)
               .HasForeignKey<Coupon>(p => p.AssetId);

            // User used coupon
            modelBuilder.Entity<UserUsedCoupon>()
                .HasKey(uc => new { uc.UserId, uc.CouponId });
            modelBuilder.Entity<Coupon>()
                .HasMany(c => c.UserUsedCoupon)
                .WithOne(uc => uc.Coupon)
                .HasForeignKey(uc => uc.CouponId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserUsedCoupon)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Import Item
            modelBuilder.Entity<ImportItem>()
                .HasKey(ii => new { ii.ProductVariantId, ii.ImportInvoiceId });
            modelBuilder.Entity<ImportInvoice>()
               .HasMany(i => i.ImportItem)
               .WithOne(ii => ii.ImportInvoice)
               .HasForeignKey(ii => ii.ImportInvoiceId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductVariant>()
                .HasMany(pv => pv.ImportItem)
                .WithOne(ii => ii.ProductVariant)
                .HasForeignKey(ii => ii.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            // Designer relationship
            modelBuilder.Entity<Asset>()
              .HasOne(p => p.Designer)
              .WithOne(p => p.Asset)
              .HasForeignKey<Designer>(p => p.AssetId);

            // RoomSpace - FurnitureType Relationship
            modelBuilder.Entity<RoomSpace>()
                .HasMany(p => p.FurnitureTypes)
                .WithOne(p => p.RoomSpace)
                .HasForeignKey(p => p.RoomSpaceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
               .HasOne(p => p.RoomSpace)
               .WithOne(p => p.Asset)
               .HasForeignKey<RoomSpace>(p => p.AssetId);

            // FurnitureType relationship
            modelBuilder.Entity<Asset>()
                .HasOne(p => p.FurnitureType)
                .WithOne(p => p.Asset)
                .HasForeignKey<FurnitureType>(p => p.AssetId);

            modelBuilder.Entity<FurnitureType>()
                .HasMany(p => p.Categories)
                .WithOne(p => p.FurnitureType)
                .HasForeignKey(p => p.FurnitureTypeId)
                .OnDelete(DeleteBehavior.Restrict);        
            
            // Material relationship
            modelBuilder.Entity<Asset>()
              .HasOne(p => p.Material)
              .WithOne(p => p.Asset)
              .HasForeignKey<Material>(p => p.AssetId);

            // Product relationship
            modelBuilder.Entity<Product>()
               .HasMany(p => p.Materials)
               .WithMany(p => p.Products)
               .UsingEntity<Dictionary<string, object>>(
                   "ProductMaterial",
                   j => j.HasOne<Material>().WithMany().HasForeignKey("MaterialId").OnDelete(DeleteBehavior.Restrict),
                   j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Restrict));


            modelBuilder.Entity<Favorite>()
                .HasKey(uc => new { uc.UserId, uc.ProductId });
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Favorites)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasMany(p => p.Favorites)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Asset>()
                 .HasOne(p => p.Product)
                 .WithOne(p => p.Asset)
                 .HasForeignKey<Product>(p => p.AssetId);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Designers)
                .WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductDesigner",
                    j => j.HasOne<Designer>().WithMany().HasForeignKey("DesignerId").OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Restrict));


            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductVariants)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
               .HasMany(p => p.Questions)
               .WithOne(p => p.Product)
               .HasForeignKey(p => p.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
            // Product Variant
            modelBuilder.Entity<ProductVariant>()
                .HasMany(p => p.Assets)
                .WithOne(p => p.ProductVariant)
                .HasForeignKey(p => p.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            // Reply relationship
            modelBuilder.Entity<Reply>()
               .HasOne(p => p.Question)
               .WithMany(p => p.Reply)
               .HasForeignKey(p => p.QuestionId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reply>()
               .HasOne(p => p.User)
               .WithMany(p => p.Reply)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
            // Question relationship
            modelBuilder.Entity<Question>()
               .HasOne(p => p.User)
               .WithMany(p => p.Question)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
            // Review relationship
            modelBuilder.Entity<Review>()
               .HasOne(p => p.User)
               .WithMany(p => p.Reviews)
               .HasForeignKey(p => p.UserId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Review>()
                .HasMany(a => a.Asset)
                .WithOne(r => r.Review)
                .HasForeignKey(a => a.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Reply>()
               .HasOne(p => p.Review)
               .WithMany(p => p.Reply)
               .HasForeignKey(p => p.ReviewId)
               .OnDelete(DeleteBehavior.SetNull);
            // Notification relationship
            modelBuilder.Entity<Notification>()
               .HasMany(p => p.Users)
               .WithMany(p => p.Notifications)
               .UsingEntity<Dictionary<string, object>>(
                   "UserNotification",
                   j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Restrict),
                   j => j.HasOne<Notification>().WithMany().HasForeignKey("NotificationId").OnDelete(DeleteBehavior.Restrict));

            // Order relationship
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.Color)
                .WithMany()
                .HasForeignKey(p => p.ColorId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
               .HasMany(p => p.OrderItems)
               .WithOne(p => p.Order)
               .HasForeignKey(p => p.OrderId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(p => p.Coupon)
                .WithMany(p => p.Orders)
                .HasForeignKey(p => p.CouponId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(p => p.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(p => p.Address)
                .WithMany(p => p.Orders)
                .HasForeignKey(p => p.AddressId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Cart>()
              .HasMany(p => p.OrderItems)
              .WithOne(p => p.Cart)
              .HasForeignKey(p => p.CartId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Cart>()
              .HasOne(p => p.User)
              .WithOne(p => p.Cart)
              .HasForeignKey<Cart>(p => p.UserId);
            modelBuilder.Entity<AspNetTypeClaims>()
               .HasMany(tc => tc.RoleClaims)
               .WithOne(rc => rc.AspNetTypeClaims)
               .HasForeignKey(rc => rc.AspNetTypeClaimsId)
               .OnDelete(DeleteBehavior.Restrict);
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Type Claims
            modelBuilder.Entity<AspNetTypeClaims>().HasData(
                new AspNetTypeClaims
                {
                    Id = 1,
                    Name = "ManageUser"
                },
                new AspNetTypeClaims
                {
                    Id = 2,
                    Name = "ManageBrand"
                },
                new AspNetTypeClaims
                {
                    Id = 3,
                    Name = "ManageCategory"
                },
                new AspNetTypeClaims
                {
                    Id = 4,
                    Name = "ManageColor"
                },
                new AspNetTypeClaims
                {
                    Id = 5,
                    Name = "ManageCoupon"
                },
                new AspNetTypeClaims
                {
                    Id = 6,
                    Name = "ManageCustomer"
                },
                new AspNetTypeClaims
                {
                    Id = 7,
                    Name = "ManageDesigner"
                },
                new AspNetTypeClaims
                {
                    Id = 8,
                    Name = "ManageFurnitureType"
                },
                new AspNetTypeClaims
                {
                    Id = 9,
                    Name = "ManageMaterial"
                },
                new AspNetTypeClaims
                {
                    Id = 10,
                    Name = "ManageMaterialType"
                },
                new AspNetTypeClaims
                {
                    Id = 11,
                    Name = "ManageNotification"
                },
                new AspNetTypeClaims
                {
                    Id = 12,
                    Name = "ManageRole"
                },
                new AspNetTypeClaims
                {
                    Id = 13,
                    Name = "ManageOrder"
                },
                new AspNetTypeClaims
                {
                    Id = 14,
                    Name = "ManageProduct"
                },
                new AspNetTypeClaims
                {
                    Id = 15,
                    Name = "ManageQuestion"
                },
                new AspNetTypeClaims
                {
                    Id = 16,
                    Name = "ManageReply"
                },
                new AspNetTypeClaims
                {
                    Id = 17,
                    Name = "ManageReview"
                },
                new AspNetTypeClaims
                {
                    Id = 18,
                    Name = "ManageRoomSpace"
                },
                new AspNetTypeClaims
                {
                    Id = 19,
                    Name = "ManageReport"
                }
            );
            // Seed roles
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1", // Unique identifier for the role
                    Name = "Owner",
                    NormalizedName = "OWNER"
                },
                new IdentityRole
                {
                    Id = "2", // Unique identifier for the role
                    Name = "Staff",
                    NormalizedName = "STAFF",
                },
                new IdentityRole
                {
                    Id = "3", // Unique identifier for the role
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                }
            );

            // Seed role claims
            modelBuilder.Entity<AspNetRoleClaims<string>>().HasData(
                // Owner permission => all
                // User claims
                new AspNetRoleClaims<string>
                {
                    Id = 1,
                    RoleId = "1", // Owner role
                    ClaimType = "CreateUser",
                    ClaimValue = "CreateUser",
                    AspNetTypeClaimsId = 1
                },
                new AspNetRoleClaims<string>
                {
                    Id = 2,
                    RoleId = "1",
                    ClaimType = "UpdateUser",
                    ClaimValue = "UpdateUser",
                    AspNetTypeClaimsId = 1
                },
                new AspNetRoleClaims<string>
                {
                    Id = 3,
                    RoleId = "1",
                    ClaimType = "DeleteUser",
                    ClaimValue = "DeleteUser",
                    AspNetTypeClaimsId = 1
                },

                // Brand claims
                new AspNetRoleClaims<string>
                {
                    Id = 4,
                    RoleId = "1",
                    ClaimType = "CreateBrand",
                    ClaimValue = "CreateBrand",
                    AspNetTypeClaimsId = 2
                },
                new AspNetRoleClaims<string>
                {
                    Id = 5,
                    RoleId = "1",
                    ClaimType = "UpdateBrand",
                    ClaimValue = "UpdateBrand",
                    AspNetTypeClaimsId = 2
                },
                new AspNetRoleClaims<string>
                {
                    Id = 6,
                    RoleId = "1",
                    ClaimType = "DeleteBrand",
                    ClaimValue = "DeleteBrand",
                    AspNetTypeClaimsId = 2
                },

                // Category claims
                new AspNetRoleClaims<string>
                {
                    Id = 7,
                    RoleId = "1",
                    ClaimType = "CreateCategory",
                    ClaimValue = "CreateCategory",
                    AspNetTypeClaimsId = 3
                },
                new AspNetRoleClaims<string>
                {
                    Id = 8,
                    RoleId = "1",
                    ClaimType = "UpdateCategory",
                    ClaimValue = "UpdateCategory",
                    AspNetTypeClaimsId = 3
                },
                new AspNetRoleClaims<string>
                {
                    Id = 9,
                    RoleId = "1",
                    ClaimType = "DeleteCategory",
                    ClaimValue = "DeleteCategory",
                    AspNetTypeClaimsId = 3
                },

                // Color claims
                new AspNetRoleClaims<string>
                {
                    Id = 10,
                    RoleId = "1",
                    ClaimType = "CreateColor",
                    ClaimValue = "CreateColor",
                    AspNetTypeClaimsId = 4
                },
                new AspNetRoleClaims<string>
                {
                    Id = 11,
                    RoleId = "1",
                    ClaimType = "UpdateColor",
                    ClaimValue = "UpdateColor",
                    AspNetTypeClaimsId = 4
                },
                new AspNetRoleClaims<string>
                {
                    Id = 12,
                    RoleId = "1",
                    ClaimType = "DeleteColor",
                    ClaimValue = "DeleteColor",
                    AspNetTypeClaimsId = 4
                },

                // Coupon claims
                new AspNetRoleClaims<string>
                {
                    Id = 13,
                    RoleId = "1",
                    ClaimType = "CreateCoupon",
                    ClaimValue = "CreateCoupon",
                    AspNetTypeClaimsId = 5
                },
                new AspNetRoleClaims<string>
                {
                    Id = 14,
                    RoleId = "1",
                    ClaimType = "UpdateCoupon",
                    ClaimValue = "UpdateCoupon",
                    AspNetTypeClaimsId = 5
                },
                new AspNetRoleClaims<string>
                {
                    Id = 15,
                    RoleId = "1",
                    ClaimType = "DeleteCoupon",
                    ClaimValue = "DeleteCoupon",
                    AspNetTypeClaimsId = 5
                },

                // Customer claims
                new AspNetRoleClaims<string>
                {
                    Id = 16,
                    RoleId = "1",
                    ClaimType = "CreateCustomer",
                    ClaimValue = "CreateCustomer",
                    AspNetTypeClaimsId = 6
                },
                new AspNetRoleClaims<string>
                {
                    Id = 17,
                    RoleId = "1",
                    ClaimType = "UpdateCustomer",
                    ClaimValue = "UpdateCustomer",
                    AspNetTypeClaimsId = 6
                },
                new AspNetRoleClaims<string>
                {
                    Id = 18,
                    RoleId = "1",
                    ClaimType = "DeleteCustomer",
                    ClaimValue = "DeleteCustomer",
                    AspNetTypeClaimsId = 6
                },

                // Designer claims
                new AspNetRoleClaims<string>
                {
                    Id = 19,
                    RoleId = "1",
                    ClaimType = "CreateDesigner",
                    ClaimValue = "CreateDesigner",
                    AspNetTypeClaimsId = 7
                },
                new AspNetRoleClaims<string>
                {
                    Id = 20,
                    RoleId = "1",
                    ClaimType = "UpdateDesigner",
                    ClaimValue = "UpdateDesigner",
                    AspNetTypeClaimsId = 7
                },
                new AspNetRoleClaims<string>
                {
                    Id = 21,
                    RoleId = "1",
                    ClaimType = "DeleteDesigner",
                    ClaimValue = "DeleteDesigner",
                    AspNetTypeClaimsId = 7
                },

                // FurnitureType claims
                new AspNetRoleClaims<string>
                {
                    Id = 22,
                    RoleId = "1",
                    ClaimType = "CreateFurnitureType",
                    ClaimValue = "CreateFurnitureType",
                    AspNetTypeClaimsId = 8
                },
                new AspNetRoleClaims<string>
                {
                    Id = 23,
                    RoleId = "1",
                    ClaimType = "UpdateFurnitureType",
                    ClaimValue = "UpdateFurnitureType",
                    AspNetTypeClaimsId = 8
                },
                new AspNetRoleClaims<string>
                {
                    Id = 24,
                    RoleId = "1",
                    ClaimType = "DeleteFurnitureType",
                    ClaimValue = "DeleteFurnitureType",
                    AspNetTypeClaimsId = 8
                },

                // Material claims
                new AspNetRoleClaims<string>
                {
                    Id = 25,
                    RoleId = "1",
                    ClaimType = "CreateMaterial",
                    ClaimValue = "CreateMaterial",
                    AspNetTypeClaimsId = 9
                },
                new AspNetRoleClaims<string>
                {
                    Id = 26,
                    RoleId = "1",
                    ClaimType = "UpdateMaterial",
                    ClaimValue = "UpdateMaterial",
                    AspNetTypeClaimsId = 9
                },
                new AspNetRoleClaims<string>
                {
                    Id = 27,
                    RoleId = "1",
                    ClaimType = "DeleteMaterial",
                    ClaimValue = "DeleteMaterial",
                    AspNetTypeClaimsId = 9
                },

                // MaterialType claims
                new AspNetRoleClaims<string>
                {
                    Id = 28,
                    RoleId = "1",
                    ClaimType = "CreateMaterialType",
                    ClaimValue = "CreateMaterialType",
                    AspNetTypeClaimsId = 10
                },
                new AspNetRoleClaims<string>
                {
                    Id = 29,
                    RoleId = "1",
                    ClaimType = "UpdateMaterialType",
                    ClaimValue = "UpdateMaterialType",
                    AspNetTypeClaimsId = 10
                },
                new AspNetRoleClaims<string>
                {
                    Id = 30,
                    RoleId = "1",
                    ClaimType = "DeleteMaterialType",
                    ClaimValue = "DeleteMaterialType",
                    AspNetTypeClaimsId = 10
                },

                // Notification claims
                new AspNetRoleClaims<string>
                {
                    Id = 31,
                    RoleId = "1",
                    ClaimType = "CreateNotification",
                    ClaimValue = "CreateNotification",
                    AspNetTypeClaimsId = 11
                },
                new AspNetRoleClaims<string>
                {
                    Id = 32,
                    RoleId = "1",
                    ClaimType = "UpdateNotification",
                    ClaimValue = "UpdateNotification",
                    AspNetTypeClaimsId = 11
                },
                new AspNetRoleClaims<string>
                {
                    Id = 33,
                    RoleId = "1",
                    ClaimType = "DeleteNotification",
                    ClaimValue = "DeleteNotification",
                    AspNetTypeClaimsId = 11
                },

                // Role claims
                new AspNetRoleClaims<string>
                {
                    Id = 34,
                    RoleId = "1",
                    ClaimType = "CreateRole",
                    ClaimValue = "CreateRole",
                    AspNetTypeClaimsId = 12
                },
                new AspNetRoleClaims<string>
                {
                    Id = 35,
                    RoleId = "1",
                    ClaimType = "UpdateRole",
                    ClaimValue = "UpdateRole",
                    AspNetTypeClaimsId = 12
                },
                new AspNetRoleClaims<string>
                {
                    Id = 36,
                    RoleId = "1",
                    ClaimType = "DeleteRole",
                    ClaimValue = "DeleteRole",
                    AspNetTypeClaimsId = 12
                },

                // Order claims
                new AspNetRoleClaims<string>
                {
                    Id = 37,
                    RoleId = "1",
                    ClaimType = "CreateOrder",
                    ClaimValue = "CreateOrder",
                    AspNetTypeClaimsId = 13
                },
                new AspNetRoleClaims<string>
                {
                    Id = 38,
                    RoleId = "1",
                    ClaimType = "UpdateOrder",
                    ClaimValue = "UpdateOrder",
                    AspNetTypeClaimsId = 13
                },
                new AspNetRoleClaims<string>
                {
                    Id = 39,
                    RoleId = "1",
                    ClaimType = "DeleteOrder",
                    ClaimValue = "DeleteOrder",
                    AspNetTypeClaimsId = 13
                },

                // Product claims
                new AspNetRoleClaims<string>
                {
                    Id = 40,
                    RoleId = "1",
                    ClaimType = "CreateProduct",
                    ClaimValue = "CreateProduct",
                    AspNetTypeClaimsId = 14
                },
                new AspNetRoleClaims<string>
                {
                    Id = 41,
                    RoleId = "1",
                    ClaimType = "UpdateProduct",
                    ClaimValue = "UpdateProduct",
                    AspNetTypeClaimsId = 14
                },
                new AspNetRoleClaims<string>
                {
                    Id = 42,
                    RoleId = "1",
                    ClaimType = "DeleteProduct",
                    ClaimValue = "DeleteProduct",
                    AspNetTypeClaimsId = 14
                },

                // Question claims
                new AspNetRoleClaims<string>
                {
                    Id = 43,
                    RoleId = "1",
                    ClaimType = "CreateQuestion",
                    ClaimValue = "CreateQuestion",
                    AspNetTypeClaimsId = 15
                },
                new AspNetRoleClaims<string>
                {
                    Id = 44,
                    RoleId = "1",
                    ClaimType = "UpdateQuestion",
                    ClaimValue = "UpdateQuestion",
                    AspNetTypeClaimsId = 15
                },
                new AspNetRoleClaims<string>
                {
                    Id = 45,
                    RoleId = "1",
                    ClaimType = "DeleteQuestion",
                    ClaimValue = "DeleteQuestion",
                    AspNetTypeClaimsId = 15
                },

                // Reply claims
                new AspNetRoleClaims<string>
                {
                    Id = 46,
                    RoleId = "1",
                    ClaimType = "CreateReply",
                    ClaimValue = "CreateReply",
                    AspNetTypeClaimsId = 16
                },
                new AspNetRoleClaims<string>
                {
                    Id = 47,
                    RoleId = "1",
                    ClaimType = "UpdateReply",
                    ClaimValue = "UpdateReply",
                    AspNetTypeClaimsId = 16
                },
                new AspNetRoleClaims<string>
                {
                    Id = 48,
                    RoleId = "1",
                    ClaimType = "DeleteReply",
                    ClaimValue = "DeleteReply",
                    AspNetTypeClaimsId = 16
                },

                // Review claims
                new AspNetRoleClaims<string>
                {
                    Id = 49,
                    RoleId = "1",
                    ClaimType = "CreateReview",
                    ClaimValue = "CreateReview",
                    AspNetTypeClaimsId = 17
                },
                new AspNetRoleClaims<string>
                {
                    Id = 50,
                    RoleId = "1",
                    ClaimType = "UpdateReview",
                    ClaimValue = "UpdateReview",
                    AspNetTypeClaimsId = 17
                },
                new AspNetRoleClaims<string>
                {
                    Id = 51,
                    RoleId = "1",
                    ClaimType = "DeleteReview",
                    ClaimValue = "DeleteReview",
                    AspNetTypeClaimsId = 17
                },

                // RoomSpace claims
                new AspNetRoleClaims<string>
                {
                    Id = 52,
                    RoleId = "1",
                    ClaimType = "CreateRoomSpace",
                    ClaimValue = "CreateRoomSpace",
                    AspNetTypeClaimsId = 18
                },
                new AspNetRoleClaims<string>
                {
                    Id = 53,
                    RoleId = "1",
                    ClaimType = "UpdateRoomSpace",
                    ClaimValue = "UpdateRoomSpace",
                    AspNetTypeClaimsId = 18
                },
                new AspNetRoleClaims<string>
                {
                    Id = 54,
                    RoleId = "1",
                    ClaimType = "DeleteRoomSpace",
                    ClaimValue = "DeleteRoomSpace",
                    AspNetTypeClaimsId = 18
                },

                // Report claims
                new AspNetRoleClaims<string>
                {
                    Id = 55,
                    RoleId = "1",
                    ClaimType = "CreateReport",
                    ClaimValue = "CreateReport",
                    AspNetTypeClaimsId = 19
                },

                // Staff permission 
                new AspNetRoleClaims<string>
                {
                    Id = 56,
                    RoleId = "2", // Updated role
                    ClaimType = "CreateUser",
                    ClaimValue = "CreateUser",
                    AspNetTypeClaimsId = 1
                },
                new AspNetRoleClaims<string>
                {
                    Id = 57,
                    RoleId = "2",
                    ClaimType = "UpdateUser",
                    ClaimValue = "UpdateUser",
                    AspNetTypeClaimsId = 1
                },
                new AspNetRoleClaims<string>
                {
                    Id = 58,
                    RoleId = "2",
                    ClaimType = "DeleteUser",
                    ClaimValue = "DeleteUser",
                    AspNetTypeClaimsId = 1
                },

                // Brand claims
                new AspNetRoleClaims<string>
                {
                    Id = 59,
                    RoleId = "2",
                    ClaimType = "CreateBrand",
                    ClaimValue = "CreateBrand",
                    AspNetTypeClaimsId = 2
                },
                new AspNetRoleClaims<string>
                {
                    Id = 60,
                    RoleId = "2",
                    ClaimType = "UpdateBrand",
                    ClaimValue = "UpdateBrand",
                    AspNetTypeClaimsId = 2
                },
                new AspNetRoleClaims<string>
                {
                    Id = 61,
                    RoleId = "2",
                    ClaimType = "DeleteBrand",
                    ClaimValue = "DeleteBrand",
                    AspNetTypeClaimsId = 2
                },

                // Category claims
                new AspNetRoleClaims<string>
                {
                    Id = 62,
                    RoleId = "2",
                    ClaimType = "CreateCategory",
                    ClaimValue = "CreateCategory",
                    AspNetTypeClaimsId = 3
                },
                new AspNetRoleClaims<string>
                {
                    Id = 63,
                    RoleId = "2",
                    ClaimType = "UpdateCategory",
                    ClaimValue = "UpdateCategory",
                    AspNetTypeClaimsId = 3
                },
                new AspNetRoleClaims<string>
                {
                    Id = 64,
                    RoleId = "2",
                    ClaimType = "DeleteCategory",
                    ClaimValue = "DeleteCategory",
                    AspNetTypeClaimsId = 3
                },

                // Color claims
                new AspNetRoleClaims<string>
                {
                    Id = 65,
                    RoleId = "2",
                    ClaimType = "CreateColor",
                    ClaimValue = "CreateColor",
                    AspNetTypeClaimsId = 4
                },
                new AspNetRoleClaims<string>
                {
                    Id = 66,
                    RoleId = "2",
                    ClaimType = "UpdateColor",
                    ClaimValue = "UpdateColor",
                    AspNetTypeClaimsId = 4
                },
                new AspNetRoleClaims<string>
                {
                    Id = 67,
                    RoleId = "2",
                    ClaimType = "DeleteColor",
                    ClaimValue = "DeleteColor",
                    AspNetTypeClaimsId = 4
                },

                // Coupon claims
                new AspNetRoleClaims<string>
                {
                    Id = 68,
                    RoleId = "2",
                    ClaimType = "CreateCoupon",
                    ClaimValue = "CreateCoupon",
                    AspNetTypeClaimsId = 5
                },
                new AspNetRoleClaims<string>
                {
                    Id = 69,
                    RoleId = "2",
                    ClaimType = "UpdateCoupon",
                    ClaimValue = "UpdateCoupon",
                    AspNetTypeClaimsId = 5
                },
                new AspNetRoleClaims<string>
                {
                    Id = 70,
                    RoleId = "2",
                    ClaimType = "DeleteCoupon",
                    ClaimValue = "DeleteCoupon",
                    AspNetTypeClaimsId = 5
                },

                // Customer claims
                new AspNetRoleClaims<string>
                {
                    Id = 71,
                    RoleId = "2",
                    ClaimType = "CreateCustomer",
                    ClaimValue = "CreateCustomer",
                    AspNetTypeClaimsId = 6
                },
                new AspNetRoleClaims<string>
                {
                    Id = 72,
                    RoleId = "2",
                    ClaimType = "UpdateCustomer",
                    ClaimValue = "UpdateCustomer",
                    AspNetTypeClaimsId = 6
                },
                new AspNetRoleClaims<string>
                {
                    Id = 73,
                    RoleId = "2",
                    ClaimType = "DeleteCustomer",
                    ClaimValue = "DeleteCustomer",
                    AspNetTypeClaimsId = 6
                },

                // Designer claims
                new AspNetRoleClaims<string>
                {
                    Id = 74,
                    RoleId = "2",
                    ClaimType = "CreateDesigner",
                    ClaimValue = "CreateDesigner",
                    AspNetTypeClaimsId = 7
                },
                new AspNetRoleClaims<string>
                {
                    Id = 75,
                    RoleId = "2",
                    ClaimType = "UpdateDesigner",
                    ClaimValue = "UpdateDesigner",
                    AspNetTypeClaimsId = 7
                },
                new AspNetRoleClaims<string>
                {
                    Id = 76,
                    RoleId = "2",
                    ClaimType = "DeleteDesigner",
                    ClaimValue = "DeleteDesigner",
                    AspNetTypeClaimsId = 7
                },

                // FurnitureType claims
                new AspNetRoleClaims<string>
                {
                    Id = 77,
                    RoleId = "2",
                    ClaimType = "CreateFurnitureType",
                    ClaimValue = "CreateFurnitureType",
                    AspNetTypeClaimsId = 8
                },
                new AspNetRoleClaims<string>
                {
                    Id = 78,
                    RoleId = "2",
                    ClaimType = "UpdateFurnitureType",
                    ClaimValue = "UpdateFurnitureType",
                    AspNetTypeClaimsId = 8
                },
                new AspNetRoleClaims<string>
                {
                    Id = 79,
                    RoleId = "2",
                    ClaimType = "DeleteFurnitureType",
                    ClaimValue = "DeleteFurnitureType",
                    AspNetTypeClaimsId = 8
                },

                // Material claims
                new AspNetRoleClaims<string>
                {
                    Id = 80,
                    RoleId = "2",
                    ClaimType = "CreateMaterial",
                    ClaimValue = "CreateMaterial",
                    AspNetTypeClaimsId = 9
                },
                new AspNetRoleClaims<string>
                {
                    Id = 81,
                    RoleId = "2",
                    ClaimType = "UpdateMaterial",
                    ClaimValue = "UpdateMaterial",
                    AspNetTypeClaimsId = 9
                },
                new AspNetRoleClaims<string>
                {
                    Id = 82,
                    RoleId = "2",
                    ClaimType = "DeleteMaterial",
                    ClaimValue = "DeleteMaterial",
                    AspNetTypeClaimsId = 9
                },

                // MaterialType claims
                new AspNetRoleClaims<string>
                {
                    Id = 83,
                    RoleId = "2",
                    ClaimType = "CreateMaterialType",
                    ClaimValue = "CreateMaterialType",
                    AspNetTypeClaimsId = 10
                },
                new AspNetRoleClaims<string>
                {
                    Id = 84,
                    RoleId = "2",
                    ClaimType = "UpdateMaterialType",
                    ClaimValue = "UpdateMaterialType",
                    AspNetTypeClaimsId = 10
                },
                new AspNetRoleClaims<string>
                {
                    Id = 85,
                    RoleId = "2",
                    ClaimType = "DeleteMaterialType",
                    ClaimValue = "DeleteMaterialType",
                    AspNetTypeClaimsId = 10
                },

                // Notification claims
                new AspNetRoleClaims<string>
                {
                    Id = 86,
                    RoleId = "2",
                    ClaimType = "CreateNotification",
                    ClaimValue = "CreateNotification",
                    AspNetTypeClaimsId = 11
                },
                new AspNetRoleClaims<string>
                {
                    Id = 87,
                    RoleId = "2",
                    ClaimType = "UpdateNotification",
                    ClaimValue = "UpdateNotification",
                    AspNetTypeClaimsId = 11
                },
                new AspNetRoleClaims<string>
                {
                    Id = 88,
                    RoleId = "2",
                    ClaimType = "DeleteNotification",
                    ClaimValue = "DeleteNotification",
                    AspNetTypeClaimsId = 11
                },

                // Role claims
                new AspNetRoleClaims<string>
                {
                    Id = 89,
                    RoleId = "2",
                    ClaimType = "CreateRole",
                    ClaimValue = "CreateRole",
                    AspNetTypeClaimsId = 12
                },
                new AspNetRoleClaims<string>
                {
                    Id = 90,
                    RoleId = "2",
                    ClaimType = "UpdateRole",
                    ClaimValue = "UpdateRole",
                    AspNetTypeClaimsId = 12
                },
                new AspNetRoleClaims<string>
                {
                    Id = 91,
                    RoleId = "2",
                    ClaimType = "DeleteRole",
                    ClaimValue = "DeleteRole",
                    AspNetTypeClaimsId = 12
                },

                // Order claims
                new AspNetRoleClaims<string>
                {
                    Id = 92,
                    RoleId = "2",
                    ClaimType = "CreateOrder",
                    ClaimValue = "CreateOrder",
                    AspNetTypeClaimsId = 13
                },
                new AspNetRoleClaims<string>
                {
                    Id = 93,
                    RoleId = "2",
                    ClaimType = "UpdateOrder",
                    ClaimValue = "UpdateOrder",
                    AspNetTypeClaimsId = 13
                },
                new AspNetRoleClaims<string>
                {
                    Id = 94,
                    RoleId = "2",
                    ClaimType = "DeleteOrder",
                    ClaimValue = "DeleteOrder",
                    AspNetTypeClaimsId = 13
                },

                // Product claims
                new AspNetRoleClaims<string>
                {
                    Id = 95,
                    RoleId = "2",
                    ClaimType = "CreateProduct",
                    ClaimValue = "CreateProduct",
                    AspNetTypeClaimsId = 14
                },
                new AspNetRoleClaims<string>
                {
                    Id = 96,
                    RoleId = "2",
                    ClaimType = "UpdateProduct",
                    ClaimValue = "UpdateProduct",
                    AspNetTypeClaimsId = 14
                },
                new AspNetRoleClaims<string>
                {
                    Id = 97,
                    RoleId = "2",
                    ClaimType = "DeleteProduct",
                    ClaimValue = "DeleteProduct",
                    AspNetTypeClaimsId = 14
                },

                // Question claims
                new AspNetRoleClaims<string>
                {
                    Id = 98,
                    RoleId = "2",
                    ClaimType = "CreateQuestion",
                    ClaimValue = "CreateQuestion",
                    AspNetTypeClaimsId = 15
                },
                new AspNetRoleClaims<string>
                {
                    Id = 99,
                    RoleId = "2",
                    ClaimType = "UpdateQuestion",
                    ClaimValue = "UpdateQuestion",
                    AspNetTypeClaimsId = 15
                },
                new AspNetRoleClaims<string>
                {
                    Id = 100,
                    RoleId = "2",
                    ClaimType = "DeleteQuestion",
                    ClaimValue = "DeleteQuestion",
                    AspNetTypeClaimsId = 15
                },

                // Reply claims
                new AspNetRoleClaims<string>
                {
                    Id = 101,
                    RoleId = "2",
                    ClaimType = "CreateReply",
                    ClaimValue = "CreateReply",
                    AspNetTypeClaimsId = 16
                },
                new AspNetRoleClaims<string>
                {
                    Id = 102,
                    RoleId = "2",
                    ClaimType = "UpdateReply",
                    ClaimValue = "UpdateReply",
                    AspNetTypeClaimsId = 16
                },
                new AspNetRoleClaims<string>
                {
                    Id = 103,
                    RoleId = "2",
                    ClaimType = "DeleteReply",
                    ClaimValue = "DeleteReply",
                    AspNetTypeClaimsId = 16
                },

                // Review claims
                new AspNetRoleClaims<string>
                {
                    Id = 104,
                    RoleId = "2",
                    ClaimType = "CreateReview",
                    ClaimValue = "CreateReview",
                    AspNetTypeClaimsId = 17
                },
                new AspNetRoleClaims<string>
                {
                    Id = 105,
                    RoleId = "2",
                    ClaimType = "UpdateReview",
                    ClaimValue = "UpdateReview",
                    AspNetTypeClaimsId = 17
                },
                new AspNetRoleClaims<string>
                {
                    Id = 106,
                    RoleId = "2",
                    ClaimType = "DeleteReview",
                    ClaimValue = "DeleteReview",
                    AspNetTypeClaimsId = 17
                },

                // RoomSpace claims
                new AspNetRoleClaims<string>
                {
                    Id = 107,
                    RoleId = "2",
                    ClaimType = "CreateRoomSpace",
                    ClaimValue = "CreateRoomSpace",
                    AspNetTypeClaimsId = 18
                },
                new AspNetRoleClaims<string>
                {
                    Id = 108,
                    RoleId = "2",
                    ClaimType = "UpdateRoomSpace",
                    ClaimValue = "UpdateRoomSpace",
                    AspNetTypeClaimsId = 18
                },
                new AspNetRoleClaims<string>
                {
                    Id = 109,
                    RoleId = "2",
                    ClaimType = "DeleteRoomSpace",
                    ClaimValue = "DeleteRoomSpace",
                    AspNetTypeClaimsId = 18
                },

                // Report claims
                new AspNetRoleClaims<string>
                {
                    Id = 110,
                    RoleId = "2",
                    ClaimType = "CreateReport",
                    ClaimValue = "CreateReport",
                    AspNetTypeClaimsId = 19
                },

                // Customer permission
                // Order claims
                new AspNetRoleClaims<string>
                {
                    Id = 111,
                    RoleId = "3",
                    ClaimType = "CreateOrder",
                    ClaimValue = "CreateOrder",
                    AspNetTypeClaimsId = 13
                },
                new AspNetRoleClaims<string>
                {
                    Id = 112,
                    RoleId = "3",
                    ClaimType = "UpdateOrder",
                    ClaimValue = "UpdateOrder",
                    AspNetTypeClaimsId = 13
                },
                new AspNetRoleClaims<string>
                {
                    Id = 113,
                    RoleId = "3",
                    ClaimType = "DeleteOrder",
                    ClaimValue = "DeleteOrder",
                    AspNetTypeClaimsId = 13
                },

                // Question claims
                new AspNetRoleClaims<string>
                {
                    Id = 114,
                    RoleId = "3",
                    ClaimType = "CreateQuestion",
                    ClaimValue = "CreateQuestion",
                    AspNetTypeClaimsId = 15
                },
                new AspNetRoleClaims<string>
                {
                    Id = 115,
                    RoleId = "3",
                    ClaimType = "UpdateQuestion",
                    ClaimValue = "UpdateQuestion",
                    AspNetTypeClaimsId = 15
                },
                new AspNetRoleClaims<string>
                {
                    Id = 116,
                    RoleId = "3",
                    ClaimType = "DeleteQuestion",
                    ClaimValue = "DeleteQuestion",
                    AspNetTypeClaimsId = 15
                },

                // Reply claims
                new AspNetRoleClaims<string>
                {
                    Id = 117,
                    RoleId = "3",
                    ClaimType = "CreateReply",
                    ClaimValue = "CreateReply",
                    AspNetTypeClaimsId = 16
                },
                new AspNetRoleClaims<string>
                {
                    Id = 118,
                    RoleId = "3",
                    ClaimType = "UpdateReply",
                    ClaimValue = "UpdateReply",
                    AspNetTypeClaimsId = 16
                },
                new AspNetRoleClaims<string>
                {
                    Id = 119,
                    RoleId = "3",
                    ClaimType = "DeleteReply",
                    ClaimValue = "DeleteReply",
                    AspNetTypeClaimsId = 16
                },

                // Review claims
                new AspNetRoleClaims<string>
                {
                    Id = 120,
                    RoleId = "3",
                    ClaimType = "CreateReview",
                    ClaimValue = "CreateReview",
                    AspNetTypeClaimsId = 17
                },
                new AspNetRoleClaims<string>
                {
                    Id = 121,
                    RoleId = "3",
                    ClaimType = "UpdateReview",
                    ClaimValue = "UpdateReview",
                    AspNetTypeClaimsId = 17
                },
                new AspNetRoleClaims<string>
                {
                    Id = 122,
                    RoleId = "3",
                    ClaimType = "DeleteReview",
                    ClaimValue = "DeleteReview",
                    AspNetTypeClaimsId = 17
                }

            );
        }
    }
}
 
