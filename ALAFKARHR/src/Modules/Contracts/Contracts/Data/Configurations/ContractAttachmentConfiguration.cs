namespace Contracts.Contracts.Data.Configurations;

public class ContractAttachmentConfiguration : IEntityTypeConfiguration<ContractAttachment>
{
    public void Configure(EntityTypeBuilder<ContractAttachment> builder)
    {
        builder.ToTable("ContractAttachments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Kind).HasConversion<int>().IsRequired();
        builder.HasIndex(x => new { x.ContractId, x.Kind });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
