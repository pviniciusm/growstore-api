using GrowStore.Domain.Entities;
using GrowStore.Domain.Entities.Accounts;
using GrowStore.Domain.Entities.Addresses;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GrowStoreDbContext).Assembly);
    }
}