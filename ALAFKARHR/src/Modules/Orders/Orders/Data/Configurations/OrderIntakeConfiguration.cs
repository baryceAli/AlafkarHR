using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Orders.Models;

namespace Orders.Data.Configurations;

public class OrderIntakeConfiguration : IEntityTypeConfiguration<OrderIntake>
{
    public void Configure(EntityTypeBuilder<OrderIntake> builder)
    {
        builder.ToTable("OrderIntakes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Number).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CustomerName).HasMaxLength(200);
        builder.Property(x => x.Channel).HasMaxLength(100);
        builder.Property(x => x.ExternalReference).HasMaxLength(100);
        builder.Property(x => x.SubmittedByUserId).HasMaxLength(100);
        builder.Property(x => x.SubmittedByCustomerId).HasMaxLength(100);
        builder.Property(x => x.PaymentMethod).HasConversion<int>();
        builder.Property(x => x.PaymentStatus).HasConversion<int>();
        builder.Property(x => x.CheckoutTotal).HasPrecision(18, 2);
        builder.Property(x => x.PaymentDecisionReason).HasMaxLength(500);
        builder.Property(x => x.RejectionReason).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.Property(x => x.Source).HasConversion<int>().IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.Status, x.SubmittedAt });
        builder.HasIndex(x => new { x.CompanyId, x.CustomerId });
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Ignore(x => x.Lines);
        builder.HasMany<OrderIntakeLine>("_lines").WithOne().OnDelete(DeleteBehavior.Cascade);
        builder.Navigation("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
