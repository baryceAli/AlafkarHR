using Catering.Models;

namespace Catering.Data.Configurations;

public class MealDefinitionConfiguration : IEntityTypeConfiguration<MealDefinition>
{
    public void Configure(EntityTypeBuilder<MealDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(180);
        builder.Property(x => x.StructureType).HasDefaultValue(CateringMealStructureType.Combo);
        builder.Property(x => x.ProductSkuName).HasMaxLength(180);
        builder.Property(x => x.ProductSkuNameEng).HasMaxLength(180);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Components).WithOne().HasForeignKey(x => x.MealDefinitionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.Name });
        builder.Navigation(x => x.Components).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class MealComponentConfiguration : IEntityTypeConfiguration<MealComponent>
{
    public void Configure(EntityTypeBuilder<MealComponent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ComponentName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ComponentNameEng).HasMaxLength(180);
        builder.Property(x => x.UnitName).HasMaxLength(80);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.QuantityPerMeal).HasColumnType("decimal(18,4)");
        builder.Property(x => x.CaloriesPerUnit).HasColumnType("decimal(18,4)");
        builder.Property(x => x.TotalCalories).HasColumnType("decimal(18,4)");
        builder.HasIndex(x => new { x.MealDefinitionId, x.ProductSkuId });
    }
}

public class CateringContractConfiguration : IEntityTypeConfiguration<CateringContract>
{
    public void Configure(EntityTypeBuilder<CateringContract> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(40).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(250).IsRequired();
        builder.Property(x => x.CustomerNameEng).HasMaxLength(250);
        builder.Property(x => x.SeasonLabel).HasMaxLength(120).IsRequired();
        builder.Property(x => x.ContractedMealQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinMealCalories).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MaxMealCalories).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Addendums).WithOne().HasForeignKey(x => x.CateringContractId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.CustomerId, x.ServiceType });
        builder.HasIndex(x => x.Number).IsUnique();
        builder.Navigation(x => x.Addendums).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CateringContractAddendumConfiguration : IEntityTypeConfiguration<CateringContractAddendum>
{
    public void Configure(EntityTypeBuilder<CateringContractAddendum> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AddedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Reason).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => new { x.CateringContractId, x.EffectiveFrom });
    }
}

public class CateringAreaConfiguration : IEntityTypeConfiguration<CateringArea>
{
    public void Configure(EntityTypeBuilder<CateringArea> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(180);
        builder.Property(x => x.GenderGroup).HasMaxLength(60);
        builder.Property(x => x.LocationText).HasMaxLength(500);
        builder.HasIndex(x => new { x.CompanyId, x.Name });
    }
}

public class CateringSquareConfiguration : IEntityTypeConfiguration<CateringSquare>
{
    public void Configure(EntityTypeBuilder<CateringSquare> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(180);
        builder.Property(x => x.LocationText).HasMaxLength(500);
        builder.Property(x => x.Latitude).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Longitude).HasColumnType("decimal(18,8)");
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.AreaId, x.Code }).IsUnique();
    }
}

public class CateringDailyScheduleConfiguration : IEntityTypeConfiguration<CateringDailySchedule>
{
    public void Configure(EntityTypeBuilder<CateringDailySchedule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlannedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Allocations).WithOne().HasForeignKey(x => x.DailyScheduleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CateringContractId, x.ServiceDate }).IsUnique();
        builder.HasIndex(x => x.CateringOperationalPlanId);
        builder.HasIndex(x => x.CateringProjectId);
        builder.HasIndex(x => x.CateringProjectDailyPlanId);
        builder.Navigation(x => x.Allocations).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CateringSquareAllocationConfiguration : IEntityTypeConfiguration<CateringSquareAllocation>
{
    public void Configure(EntityTypeBuilder<CateringSquareAllocation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlannedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ReceivedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DistributedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.ReceivingSupervisorName).HasMaxLength(180);
        builder.Property(x => x.TeamLeaderName).HasMaxLength(180);
        builder.Property(x => x.VarianceNotes).HasMaxLength(500);
        builder.HasIndex(x => new { x.DailyScheduleId, x.SquareId }).IsUnique();
    }
}

public class CateringOperationalPlanConfiguration : IEntityTypeConfiguration<CateringOperationalPlan>
{
    public void Configure(EntityTypeBuilder<CateringOperationalPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Resources).WithOne().HasForeignKey(x => x.CateringOperationalPlanId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.CateringContractId });
        builder.Navigation(x => x.Resources).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CateringPlanResourceAssignmentConfiguration : IEntityTypeConfiguration<CateringPlanResourceAssignment>
{
    public void Configure(EntityTypeBuilder<CateringPlanResourceAssignment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeName).HasMaxLength(180);
        builder.Property(x => x.VehicleName).HasMaxLength(180);
        builder.Property(x => x.PlateNumber).HasMaxLength(40);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.CateringOperationalPlanId, x.ResourceType });
    }
}

public class CateringProjectConfiguration : IEntityTypeConfiguration<CateringProject>
{
    public void Configure(EntityTypeBuilder<CateringProject> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProjectName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Contracts).WithOne().HasForeignKey(x => x.CateringProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Squares).WithOne().HasForeignKey(x => x.CateringProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.DailyPlans).WithOne().HasForeignKey(x => x.CateringProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.ProjectName });
        builder.Navigation(x => x.Contracts).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.Squares).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(x => x.DailyPlans).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CateringProjectContractLinkConfiguration : IEntityTypeConfiguration<CateringProjectContractLink>
{
    public void Configure(EntityTypeBuilder<CateringProjectContractLink> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.CateringProjectId, x.CateringContractId }).IsUnique();
    }
}

public class CateringProjectSquareScopeConfiguration : IEntityTypeConfiguration<CateringProjectSquareScope>
{
    public void Configure(EntityTypeBuilder<CateringProjectSquareScope> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.CateringProjectId, x.SquareId }).IsUnique();
    }
}

public class CateringProjectDailyPlanConfiguration : IEntityTypeConfiguration<CateringProjectDailyPlan>
{
    public void Configure(EntityTypeBuilder<CateringProjectDailyPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PlannedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CateringProjectId, x.ServiceDate }).IsUnique();
    }
}

public class CateringPackagingPlanConfiguration : IEntityTypeConfiguration<CateringPackagingPlan>
{
    public void Configure(EntityTypeBuilder<CateringPackagingPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequiredMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.StockReleasedMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PreparedMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.RejectedMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DamagedMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.InventoryReferenceIdsCsv).HasMaxLength(2000);
        builder.Property(x => x.VarianceReason).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => x.DailyScheduleId).IsUnique();
    }
}

public class CateringInventoryRequestConfiguration : IEntityTypeConfiguration<CateringInventoryRequest>
{
    public void Configure(EntityTypeBuilder<CateringInventoryRequest> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RequestedByEmployeeName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.PlannedMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.InventoryReferenceIdsCsv).HasMaxLength(2000);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.CateringInventoryRequestId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.CompanyId, x.DailyScheduleId });
        builder.HasIndex(x => new { x.SourceWarehouseId, x.Status });
        builder.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class CateringInventoryRequestLineConfiguration : IEntityTypeConfiguration<CateringInventoryRequestLine>
{
    public void Configure(EntityTypeBuilder<CateringInventoryRequestLine> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductSkuName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ProductSkuNameEng).HasMaxLength(180);
        builder.Property(x => x.QuantityPerMeal).HasColumnType("decimal(18,4)");
        builder.Property(x => x.RequiredQuantity).HasColumnType("decimal(18,4)");
        builder.Property(x => x.ApprovedQuantity).HasColumnType("decimal(18,4)");
        builder.Property(x => x.UnitName).HasMaxLength(80);
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.HasIndex(x => new { x.CateringInventoryRequestId, x.ProductSkuId });
    }
}

public class CateringDispatchPlanConfiguration : IEntityTypeConfiguration<CateringDispatchPlan>
{
    public void Configure(EntityTypeBuilder<CateringDispatchPlan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VehicleName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.PlateNumber).HasMaxLength(40);
        builder.Property(x => x.DriverName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.LoadedMealCount).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => x.DailyScheduleId).IsUnique();
        builder.HasIndex(x => new { x.VehicleId, x.Status });
    }
}

public class CateringExecutionEventConfiguration : IEntityTypeConfiguration<CateringExecutionEvent>
{
    public void Configure(EntityTypeBuilder<CateringExecutionEvent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.EmployeeName).HasMaxLength(180);
        builder.Property(x => x.LocationText).HasMaxLength(500);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.DailyScheduleId, x.OccurredAt });
        builder.HasIndex(x => new { x.AllocationId, x.EventType });
    }
}

public class CateringVehicleDeliveryConfiguration : IEntityTypeConfiguration<CateringVehicleDelivery>
{
    public void Configure(EntityTypeBuilder<CateringVehicleDelivery> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VehicleName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.PlateNumber).HasMaxLength(40);
        builder.Property(x => x.DriverName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ReceivingSupervisorName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.ReceivedQuantity).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.DailyScheduleId, x.VehicleId });
    }
}

public class CateringAssignmentConfiguration : IEntityTypeConfiguration<CateringAssignment>
{
    public void Configure(EntityTypeBuilder<CateringAssignment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmployeeName).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);
        builder.Property(x => x.CoveredSquareIdsCsv).HasMaxLength(2000);
        builder.Property(x => x.DistributorEmployeeIdsCsv).HasMaxLength(2000);
        builder.HasIndex(x => new { x.CateringContractId, x.Role, x.EmployeeId });
    }
}
