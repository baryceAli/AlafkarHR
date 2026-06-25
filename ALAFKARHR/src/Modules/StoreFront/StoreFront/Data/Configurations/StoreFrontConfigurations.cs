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
        builder.Property(x => x.AdministrationId).IsRequired(false);
        builder.Property(x => x.DepartmentId).IsRequired(false);

        builder.HasOne(x => x.StoreFrontType)
            .WithMany()
            .HasForeignKey(x => x.StoreFrontTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.SellableItems)
            .WithOne(x => x.StoreFront)
            .HasForeignKey(x => x.StoreFrontId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Departments)
            .WithOne(x => x.StoreFront)
            .HasForeignKey(x => x.StoreFrontId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.IsActive });
        builder.HasIndex(x => x.AdministrationId);
        builder.HasIndex(x => x.DepartmentId);
    }
}

public class StoreFrontDepartmentConfiguration : IEntityTypeConfiguration<StoreFrontDepartment>
{
    public void Configure(EntityTypeBuilder<StoreFrontDepartment> builder)
    {
        builder.ToTable("StoreFrontDepartments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.StoreFrontId, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.StoreFrontId, x.IsActive });
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

public class PosCashierSessionConfiguration : IEntityTypeConfiguration<PosCashierSession>
{
    public void Configure(EntityTypeBuilder<PosCashierSession> builder)
    {
        builder.ToTable("PosCashierSessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CashierUserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OpeningAmount).HasPrecision(18, 2);
        builder.Property(x => x.ExpectedCashAmount).HasPrecision(18, 2);
        builder.Property(x => x.CashSalesAmount).HasPrecision(18, 2);
        builder.Property(x => x.CardSalesAmount).HasPrecision(18, 2);
        builder.Property(x => x.CountedCashAmount).HasPrecision(18, 2);
        builder.Property(x => x.VarianceAmount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.StoreFrontId });
        builder.HasIndex(x => new { x.StoreFrontId, x.CashierUserId, x.Status });
    }
}

public class PosCashierSessionTransferConfiguration : IEntityTypeConfiguration<PosCashierSessionTransfer>
{
    public void Configure(EntityTypeBuilder<PosCashierSessionTransfer> builder)
    {
        builder.ToTable("PosCashierSessionTransfers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.CreatedBy).HasMaxLength(100);
        builder.Property(x => x.ModifiedBy).HasMaxLength(100);
        builder.Property(x => x.DeletedBy).HasMaxLength(100);
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.StoreFrontId });
        builder.HasIndex(x => x.FromSessionId);
        builder.HasIndex(x => x.ToSessionId);
    }
}
