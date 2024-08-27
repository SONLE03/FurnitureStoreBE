using FurnitureStoreBE.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<RefreshToken> Tokens { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Designer> Designer { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<RoomSpace> RoomSpaces { get; set; }
        public DbSet<FurnitureType> FurnitureTypes  { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Reply> Replys { get; set; }
        public DbSet<UserUsedCoupon> UserUsedCoupon { get; set; }   
        public DbSet<Notification> Notification { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // User Relationships
            modelBuilder.Entity<Asset>()
                .HasOne(p => p.User)
                .WithOne(p => p.Asset)
                .HasForeignKey<User>(p => p.AssetId);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Tokens)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Admin>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Admin>(p => p.id);

            modelBuilder.Entity<Customer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(p => p.id);

            modelBuilder.Entity<Staff>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Staff>(p => p.id);

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

            // MaterialType relationship
            modelBuilder.Entity<Asset>()
              .HasOne(p => p.MaterialType)
              .WithOne(p => p.Asset)
              .HasForeignKey<MaterialType>(p => p.AssetId);

            modelBuilder.Entity<MaterialType>()
              .HasMany(p => p.Materials)
              .WithOne(p => p.MaterialType)
              .HasForeignKey(p => p.MaterialTypeId)
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
                .HasMany(p => p.Coupons)
                .WithMany(p => p.ProductApplied)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductApplied",
                    j => j.HasOne<Coupon>().WithMany().HasForeignKey("CouponId").OnDelete(DeleteBehavior.Restrict),
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
                .HasOne(p => p.Review)
                .WithMany(p => p.Reply)
                .HasForeignKey(p => p.ReviewId)
                .OnDelete(DeleteBehavior.Restrict);
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
            // Notification relationship
            modelBuilder.Entity<Notification>()
               .HasMany(p => p.Users)
               .WithMany(p => p.Notifications)
               .UsingEntity<Dictionary<string, object>>(
                   "UserNotification",
                   j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Restrict),
                   j => j.HasOne<Notification>().WithMany().HasForeignKey("NotificationId").OnDelete(DeleteBehavior.Restrict));

            // Order relationship
            modelBuilder.Entity<OrderItem>()
               .HasKey(uc => new { uc.OrderId, uc.ProductVariantId });
            modelBuilder.Entity<ProductVariant>()
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.ProductVariant)
                .HasForeignKey(p => p.ProductVariantId)
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
            // Cart relationship
            modelBuilder.Entity<CartItem>()
             .HasKey(uc => new { uc.CartId, uc.ProductVariantId });
            modelBuilder.Entity<ProductVariant>()
                .HasMany(p => p.CartItems)
                .WithOne(p => p.ProductVariant)
                .HasForeignKey(p => p.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Cart>()
                .HasMany(p => p.CartItems)
                .WithOne(p => p.Cart)
                .HasForeignKey(p => p.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
              .HasOne(p => p.User)
              .WithOne(p => p.Cart)
              .HasForeignKey<Cart>(p => p.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }

}
