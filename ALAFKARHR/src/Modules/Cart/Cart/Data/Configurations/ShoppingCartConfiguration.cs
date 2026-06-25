using Cart.Carts.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.Data.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.ToTable("Carts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).HasMaxLength(100);
        builder.Property(x => x.SessionId).HasMaxLength(100);
        builder.Property(x => x.Channel).HasMaxLength(100);
        builder.Property(x => x.Source).HasConversion<int>().IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.UserId, x.SessionId });
        builder.HasIndex(x => new { x.CompanyId, x.BranchId });
        builder.HasIndex(x => new { x.StoreFrontId, x.PosCashierSessionId });
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Ignore(x => x.Lines);
        builder.HasMany<ShoppingCartLine>("_lines").WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Navigation("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
