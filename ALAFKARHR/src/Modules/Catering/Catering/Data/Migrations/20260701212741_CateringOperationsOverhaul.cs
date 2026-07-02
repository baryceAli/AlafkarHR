using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Data.Migrations
{
    /// <inheritdoc />
    public partial class CateringOperationsOverhaul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualArrivalTime",
                schema: "Catering",
                table: "CateringSquareAllocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedArrivalTime",
                schema: "Catering",
                table: "CateringSquareAllocations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceivingSupervisorEmployeeId",
                schema: "Catering",
                table: "CateringSquareAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivingSupervisorName",
                schema: "Catering",
                table: "CateringSquareAllocations",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamLeaderEmployeeId",
                schema: "Catering",
                table: "CateringSquareAllocations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TeamLeaderName",
                schema: "Catering",
                table: "CateringSquareAllocations",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedArrivalTime",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedDepartureTime",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedLoadTime",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedPackagingEndTime",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedPackagingStartTime",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultPackagingWarehouseId",
                schema: "Catering",
                table: "CateringContracts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultSourceWarehouseId",
                schema: "Catering",
                table: "CateringContracts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPackagingRequired",
                schema: "Catering",
                table: "CateringContracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CateringDispatchPlans",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    DriverEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    FleetAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsFleetAssignmentManagedByCatering = table.Column<bool>(type: "bit", nullable: false),
                    LoadedMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlannedLoadTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedDepartureTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlannedArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TruckArrivedForLoadingAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoadedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArrivedAtDistributionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CateringDispatchPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringExecutionEvents",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DispatchPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    LocationText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CateringExecutionEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringPackagingPlans",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPackagingRequired = table.Column<bool>(type: "bit", nullable: false),
                    SourceWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackagingWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequiredMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockReleasedMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreparedMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RejectedMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DamagedMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StockReleasedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreparationStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreparationCompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InventoryReferenceIdsCsv = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    VarianceReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CateringPackagingPlans", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CateringDispatchPlans_DailyScheduleId",
                schema: "Catering",
                table: "CateringDispatchPlans",
                column: "DailyScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringDispatchPlans_VehicleId_Status",
                schema: "Catering",
                table: "CateringDispatchPlans",
                columns: new[] { "VehicleId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringExecutionEvents_AllocationId_EventType",
                schema: "Catering",
                table: "CateringExecutionEvents",
                columns: new[] { "AllocationId", "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringExecutionEvents_DailyScheduleId_OccurredAt",
                schema: "Catering",
                table: "CateringExecutionEvents",
                columns: new[] { "DailyScheduleId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringPackagingPlans_DailyScheduleId",
                schema: "Catering",
                table: "CateringPackagingPlans",
                column: "DailyScheduleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CateringDispatchPlans",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringExecutionEvents",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringPackagingPlans",
                schema: "Catering");

            migrationBuilder.DropColumn(
                name: "ActualArrivalTime",
                schema: "Catering",
                table: "CateringSquareAllocations");

            migrationBuilder.DropColumn(
                name: "PlannedArrivalTime",
                schema: "Catering",
                table: "CateringSquareAllocations");

            migrationBuilder.DropColumn(
                name: "ReceivingSupervisorEmployeeId",
                schema: "Catering",
                table: "CateringSquareAllocations");

            migrationBuilder.DropColumn(
                name: "ReceivingSupervisorName",
                schema: "Catering",
                table: "CateringSquareAllocations");

            migrationBuilder.DropColumn(
                name: "TeamLeaderEmployeeId",
                schema: "Catering",
                table: "CateringSquareAllocations");

            migrationBuilder.DropColumn(
                name: "TeamLeaderName",
                schema: "Catering",
                table: "CateringSquareAllocations");

            migrationBuilder.DropColumn(
                name: "PlannedArrivalTime",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "PlannedDepartureTime",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "PlannedLoadTime",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "PlannedPackagingEndTime",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "PlannedPackagingStartTime",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "DefaultPackagingWarehouseId",
                schema: "Catering",
                table: "CateringContracts");

            migrationBuilder.DropColumn(
                name: "DefaultSourceWarehouseId",
                schema: "Catering",
                table: "CateringContracts");

            migrationBuilder.DropColumn(
                name: "IsPackagingRequired",
                schema: "Catering",
                table: "CateringContracts");
        }
    }
}
