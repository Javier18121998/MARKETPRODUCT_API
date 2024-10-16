using MARKETPRODUCT_API.Data.EFModels;
using Microsoft.EntityFrameworkCore;

namespace MARKETPRODUCT_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar la relación uno a muchos entre Order y OrderItem
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Configurar la relación uno a muchos entre Product y OrderItem
            modelBuilder.Entity<Product>()
                .HasMany<OrderItem>()
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);
        }
    }

}
