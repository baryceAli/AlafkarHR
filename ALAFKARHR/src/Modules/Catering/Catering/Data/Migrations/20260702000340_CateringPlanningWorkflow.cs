using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Data.Migrations
{
    /// <inheritdoc />
    public partial class CateringPlanningWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "Catering",
                table: "MealDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductPackageId",
                schema: "Catering",
                table: "MealDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductSkuId",
                schema: "Catering",
                table: "MealDefinitions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductSkuName",
                schema: "Catering",
                table: "MealDefinitions",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductSkuNameEng",
                schema: "Catering",
                table: "MealDefinitions",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StructureType",
                schema: "Catering",
                table: "MealDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "CateringOperationalPlanId",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CateringInventoryRequests",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CateringOperationalPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DailyScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackagingPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceWarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByEmployeeName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedMealCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    InventoryReferenceIdsCsv = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
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
                    table.PrimaryKey("PK_CateringInventoryRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringOperationalPlans",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CateringContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CateringOperationalPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringInventoryRequestLines",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringInventoryRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    ProductSkuNameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    QuantityPerMeal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    RequiredQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ApprovedQuantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_CateringInventoryRequestLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringInventoryRequestLines_CateringInventoryRequests_CateringInventoryRequestId",
                        column: x => x.CateringInventoryRequestId,
                        principalSchema: "Catering",
                        principalTable: "CateringInventoryRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CateringPlanResourceAssignments",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringOperationalPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResourceType = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VehicleName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    SquareId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_CateringPlanResourceAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringPlanResourceAssignments_CateringOperationalPlans_CateringOperationalPlanId",
                        column: x => x.CateringOperationalPlanId,
                        principalSchema: "Catering",
                        principalTable: "CateringOperationalPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CateringDailySchedules_CateringOperationalPlanId",
                schema: "Catering",
                table: "CateringDailySchedules",
                column: "CateringOperationalPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CateringInventoryRequestLines_CateringInventoryRequestId_ProductSkuId",
                schema: "Catering",
                table: "CateringInventoryRequestLines",
                columns: new[] { "CateringInventoryRequestId", "ProductSkuId" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringInventoryRequests_CompanyId_DailyScheduleId",
                schema: "Catering",
                table: "CateringInventoryRequests",
                columns: new[] { "CompanyId", "DailyScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringInventoryRequests_SourceWarehouseId_Status",
                schema: "Catering",
                table: "CateringInventoryRequests",
                columns: new[] { "SourceWarehouseId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringOperationalPlans_CompanyId_CateringContractId",
                schema: "Catering",
                table: "CateringOperationalPlans",
                columns: new[] { "CompanyId", "CateringContractId" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringPlanResourceAssignments_CateringOperationalPlanId_ResourceType",
                schema: "Catering",
                table: "CateringPlanResourceAssignments",
                columns: new[] { "CateringOperationalPlanId", "ResourceType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CateringInventoryRequestLines",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringPlanResourceAssignments",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringInventoryRequests",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringOperationalPlans",
                schema: "Catering");

            migrationBuilder.DropIndex(
                name: "IX_CateringDailySchedules_CateringOperationalPlanId",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "Catering",
                table: "MealDefinitions");

            migrationBuilder.DropColumn(
                name: "ProductPackageId",
                schema: "Catering",
                table: "MealDefinitions");

            migrationBuilder.DropColumn(
                name: "ProductSkuId",
                schema: "Catering",
                table: "MealDefinitions");

            migrationBuilder.DropColumn(
                name: "ProductSkuName",
                schema: "Catering",
                table: "MealDefinitions");

            migrationBuilder.DropColumn(
                name: "ProductSkuNameEng",
                schema: "Catering",
                table: "MealDefinitions");

            migrationBuilder.DropColumn(
                name: "StructureType",
                schema: "Catering",
                table: "MealDefinitions");

            migrationBuilder.DropColumn(
                name: "CateringOperationalPlanId",
                schema: "Catering",
                table: "CateringDailySchedules");
        }
    }
}
