using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectManagement.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProjectNumber).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(250);
        builder.Property(x => x.ManagerName).HasMaxLength(180);
        builder.Property(x => x.SourceOrderNumber).HasMaxLength(80);
        builder.Property(x => x.SourceOrderType).HasMaxLength(80);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.ProjectNumber }).IsUnique();
        builder.HasMany(x => x.Customers).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Deliverables).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.DistributionSchedules).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.MaterialRequirements).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Resources).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Expenses).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Handoffs).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.TaskLinks).WithOne().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProjectCustomerConfiguration : IEntityTypeConfiguration<ProjectCustomer>
{
    public void Configure(EntityTypeBuilder<ProjectCustomer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.CustomerNameEng).HasMaxLength(250);
        builder.Property(x => x.SourceOrderNumber).HasMaxLength(80);
        builder.Property(x => x.ContractedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ContractedAmount).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.ProductPlans).WithOne().HasForeignKey(x => x.ProjectCustomerId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ProjectCustomerProductPlanConfiguration : IEntityTypeConfiguration<ProjectCustomerProductPlan>
{
    public void Configure(EntityTypeBuilder<ProjectCustomerProductPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductSkuName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.ProductSkuNameEng).HasMaxLength(250);
        builder.Property(x => x.SkuCode).HasMaxLength(80);
        builder.Property(x => x.SkuCodeEng).HasMaxLength(80);
        builder.Property(x => x.PackageName).HasMaxLength(180);
        builder.Property(x => x.PackageNameEng).HasMaxLength(180);
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.ProjectId, x.ProjectCustomerId, x.ProductSkuId });
    }
}

public class ProjectDeliverableConfiguration : IEntityTypeConfiguration<ProjectDeliverable>
{
    public void Configure(EntityTypeBuilder<ProjectDeliverable> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductSkuName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.ProductSkuNameEng).HasMaxLength(250);
        builder.Property(x => x.OrderedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.PlannedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ProducedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ShippedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
    }
}

public class DistributionPlaceConfiguration : IEntityTypeConfiguration<DistributionPlace>
{
    public void Configure(EntityTypeBuilder<DistributionPlace> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(250);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(120);
        builder.Property(x => x.ContactName).HasMaxLength(180);
        builder.Property(x => x.ContactPhone).HasMaxLength(60);
        builder.Property(x => x.Latitude).HasPrecision(12, 8);
        builder.Property(x => x.Longitude).HasPrecision(12, 8);
        builder.HasIndex(x => new { x.CompanyId, x.Name });
    }
}

public class ProjectDistributionScheduleConfiguration : IEntityTypeConfiguration<ProjectDistributionSchedule>
{
    public void Configure(EntityTypeBuilder<ProjectDistributionSchedule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Allocations).WithOne().HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.ProjectId, x.DistributionDate });
    }
}

public class ProjectDistributionAllocationConfiguration : IEntityTypeConfiguration<ProjectDistributionAllocation>
{
    public void Configure(EntityTypeBuilder<ProjectDistributionAllocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.DeliverableName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PlaceName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PlannedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ShippedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.DeliveredQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ActualQuantity).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.ProjectId, x.DistributionDate, x.ProjectCustomerId, x.DistributionPlaceId, x.DeliverableId });
    }
}

public class ProjectMaterialRequirementConfiguration : IEntityTypeConfiguration<ProjectMaterialRequirement>
{
    public void Configure(EntityTypeBuilder<ProjectMaterialRequirement> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ComponentSkuName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.ComponentSkuNameEng).HasMaxLength(250);
        builder.Property(x => x.RequiredQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ReservedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.IssuedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ConsumedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ReturnedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.VarianceQuantity).HasPrecision(18, 4);
    }
}

public class ProjectCostConfiguration : IEntityTypeConfiguration<ProjectResource>, IEntityTypeConfiguration<ProjectExpense>
{
    public void Configure(EntityTypeBuilder<ProjectResource> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PlannedQuantity).HasPrecision(18, 4);
        builder.Property(x => x.PlannedRate).HasPrecision(18, 4);
        builder.Property(x => x.ActualQuantity).HasPrecision(18, 4);
        builder.Property(x => x.ActualRate).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
    }

    public void Configure(EntityTypeBuilder<ProjectExpense> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PlannedAmount).HasPrecision(18, 4);
        builder.Property(x => x.ActualAmount).HasPrecision(18, 4);
        builder.Property(x => x.Notes).HasMaxLength(1000);
    }
}

public class ProjectHandoffConfiguration : IEntityTypeConfiguration<ProjectHandoff>, IEntityTypeConfiguration<ProjectHandoffLine>, IEntityTypeConfiguration<ProjectTaskLink>
{
    public void Configure(EntityTypeBuilder<ProjectHandoff> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReferenceNumber).HasMaxLength(80).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.HandoffId).OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<ProjectHandoffLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ItemName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Quantity).HasPrecision(18, 4);
        builder.Property(x => x.UnitCost).HasPrecision(18, 4);
        builder.Property(x => x.TotalCost).HasPrecision(18, 4);
    }

    public void Configure(EntityTypeBuilder<ProjectTaskLink> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.TaskNumber).HasMaxLength(80);
    }
}
