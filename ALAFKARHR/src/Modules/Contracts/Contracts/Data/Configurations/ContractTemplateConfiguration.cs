namespace Contracts.Contracts.Data.Configurations;

public class ContractTemplateConfiguration : IEntityTypeConfiguration<ContractTemplate>
{
    public void Configure(EntityTypeBuilder<ContractTemplate> builder)
    {
        builder.ToTable("ContractTemplates");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ContractType).HasMaxLength(80).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(260);
        builder.Property(x => x.FilePath).HasMaxLength(500);
        builder.Property(x => x.ContentType).HasMaxLength(120);
        builder.HasIndex(x => new { x.CompanyId, x.ContractType, x.Name });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
