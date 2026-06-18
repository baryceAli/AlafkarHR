namespace Contracts.Contracts.Data.Configurations;

public class ContractRenewalConfiguration : IEntityTypeConfiguration<ContractRenewal>
{
    public void Configure(EntityTypeBuilder<ContractRenewal> builder)
    {
        builder.ToTable("ContractRenewals");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.PaymentStatus).HasConversion<int>().IsRequired();
        builder.Property(x => x.FeeAmount).HasPrecision(18, 2);
        builder.Property(x => x.PaidAmount).HasPrecision(18, 2);
        builder.Property(x => x.AccountingDocumentNumber).HasMaxLength(80);
        builder.HasIndex(x => new { x.ContractId, x.Status, x.PaymentStatus });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
