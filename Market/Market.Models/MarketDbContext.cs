using Market.DataModels.DTos;
using Market.DataModels.EFModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Market.Market.Models
{
    public class MarketDbContext : DbContext
    {
        public DbSet<ProductDto> Products { get; set; }
        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerSession> CustomerSessions { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<CustomerDataRegistrationDto> customerData { get; set; }

        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<CustomerSession>()
                .HasKey(cs => cs.Id);

            modelBuilder.Entity<CustomerSession>()
                .HasOne(cs => cs.Customer)
                .WithMany()
                .HasForeignKey(cs => cs.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany()
                .HasForeignKey(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasKey(ci => ci.Id);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
