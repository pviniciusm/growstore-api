using GrowStore.Domain.Entities;
using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using GrowStore.Domain.Entities.Orders;

namespace GrowStore.Infrastructure.Data;

public class GrowStoreDbContext : DbContext
{
    public GrowStoreDbContext(DbContextOptions<GrowStoreDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GrowStoreDbContext).Assembly);
    }
}