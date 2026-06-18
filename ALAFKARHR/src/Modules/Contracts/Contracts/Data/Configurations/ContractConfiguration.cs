namespace Contracts.Contracts.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("Contracts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TitleEng).HasMaxLength(200);
        builder.Property(x => x.Type).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.PartyType).HasMaxLength(80).IsRequired();
        builder.Property(x => x.PartyDisplayName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.ContractValue).HasPrecision(18, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.OwnsOne(x => x.RenewalSettings, renewal =>
        {
            renewal.Property(x => x.RenewalTermUnit).HasConversion<int>();
            renewal.Property(x => x.FeeMode).HasConversion<int>();
            renewal.Property(x => x.FeeAmount).HasPrecision(18, 2);
            renewal.Property(x => x.FeePercentage).HasPrecision(9, 4);
        });

        builder.HasIndex(x => new { x.CompanyId, x.Type, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.PartyType, x.PartyId, x.Status });
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasMany(x => x.Renewals)
            .WithOne()
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Attachments)
            .WithOne()
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.StatusHistory)
            .WithOne()
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Renewals).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.StatusHistory).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
