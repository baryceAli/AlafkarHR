using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.Property(rt => rt.CreatedByIp)
            .IsRequired();

        builder.Property(rt => rt.ExpiryDate)
            .IsRequired();

        // 🔹 Shadow FK (since not in model)
        builder.Property<Guid>("UserId");

        // Optional indexes (recommended 🚀)
        builder.HasIndex(rt => rt.Token)
            .IsUnique();
    }
}
