namespace Contracts.Contracts.Data.Configurations;

public class ContractStatusHistoryConfiguration : IEntityTypeConfiguration<ContractStatusHistory>
{
    public void Configure(EntityTypeBuilder<ContractStatusHistory> builder)
    {
        builder.ToTable("ContractStatusHistory");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OldStatus).HasConversion<int?>();
        builder.Property(x => x.NewStatus).HasConversion<int>().IsRequired();
        builder.Property(x => x.Action).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.ChangedBy).HasMaxLength(100).IsRequired();
        builder.HasIndex(x => new { x.ContractId, x.ChangedAt });
    }
}
