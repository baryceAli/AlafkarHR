using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // 🔹 Identity already configures Id, UserName, Email, etc.

        builder.Property(u => u.UserType)
            .IsRequired();

        builder.Property(u => u.Otp)
            .HasMaxLength(10);

        builder
       .HasMany(u => u.RefreshTokens)
       .WithOne()
       .HasForeignKey("UserId")
       .OnDelete(DeleteBehavior.Cascade);


        // 🔥 IMPORTANT: Tell EF to use field instead of property
        builder.Metadata
            .FindNavigation(nameof(ApplicationUser.RefreshTokens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}