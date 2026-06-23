namespace StoreFront.Data.Configurations;

public class StoreFrontTypeConfiguration : IEntityTypeConfiguration<StoreFrontType>
{
    public void Configure(EntityTypeBuilder<StoreFrontType> builder)
    {
        builder.ToTable("StoreFrontTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
    }
}

public class StoreFrontStoreConfiguration : IEntityTypeConfiguration<StoreFrontStore>
{
    public void Configure(EntityTypeBuilder<StoreFrontStore> builder)
    {
        builder.ToTable("StoreFronts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ReceiptHeader).HasMaxLength(1000);
        builder.Property(x => x.ReceiptFooter).HasMaxLength(1000);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasOne(x => x.StoreFrontType)
            .WithMany()
            .HasForeignKey(x => x.StoreFrontTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.SellableItems)
            .WithOne(x => x.StoreFront)
            .HasForeignKey(x => x.StoreFrontId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.IsActive });
    }
}

public class StoreFrontSellableItemConfiguration : IEntityTypeConfiguration<StoreFrontSellableItem>
{
    public void Configure(EntityTypeBuilder<StoreFrontSellableItem> builder)
    {
        builder.ToTable("StoreFrontSellableItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductName).HasMaxLength(300);
        builder.Property(x => x.ProductNameEng).HasMaxLength(300);
        builder.Property(x => x.SkuCode).HasMaxLength(100);
        builder.Property(x => x.MinimumManualPrice).HasPrecision(18, 2);
        builder.Property(x => x.MaximumManualPrice).HasPrecision(18, 2);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.StoreFrontId, x.ProductSkuId }).IsUnique();
        builder.HasIndex(x => x.ProductSkuId);
    }
}
