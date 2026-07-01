using GrowStore.Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowStore.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.AccountId)
            .IsRequired();

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(rt => rt.Token);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        builder.Property(rt => rt.RevokedAt);

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(rt => rt.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}