using GrowStore.Domain.Entities;
using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Entities.Addresses;
using GrowStore.Domain.Entities.Carts;
using GrowStore.Domain.Entities.Categories;
using GrowStore.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

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
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GrowStoreDbContext).Assembly);
    }
}