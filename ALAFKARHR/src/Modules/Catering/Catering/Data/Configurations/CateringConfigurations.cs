using Catering.Models;

namespace Catering.Data.Configurations;

public class MealDefinitionConfiguration : IEntityTypeConfiguration<MealDefinition>
{
    public void Configure(EntityTypeBuilder<MealDefinition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.NameEng).HasMaxLength(180);
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
        builder.Property(x => x.VarianceNotes).HasMaxLength(500);
        builder.HasIndex(x => new { x.DailyScheduleId, x.SquareId }).IsUnique();
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
