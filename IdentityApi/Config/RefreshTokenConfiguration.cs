using IdentityApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityApi.Config;

internal sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(refreshToken => refreshToken.Id);
        builder.Property(refreshToken => refreshToken.TokenHash).HasMaxLength(200);
        builder.HasIndex(refreshToken => refreshToken.TokenHash).IsUnique();
    }
}