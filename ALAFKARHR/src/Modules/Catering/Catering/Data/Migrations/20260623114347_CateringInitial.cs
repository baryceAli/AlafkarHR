using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Data.Migrations
{
    /// <inheritdoc />
    public partial class CateringInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catering");

            migrationBuilder.CreateTable(
                name: "CateringAreas",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    GenderGroup = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    LocationText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CateringAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringAssignments",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    SquareId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CoveredSquareIdsCsv = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DistributorEmployeeIdsCsv = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
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
                    table.PrimaryKey("PK_CateringAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringContracts",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CustomerNameEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    GenericContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    SeasonLabel = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    RamadanYear = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractedMealQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MealDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_CateringContracts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringDailySchedules",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_CateringDailySchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringSquares",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    LocationText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,8)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_CateringSquares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringVehicleDeliveries",
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
                    ReceivingSupervisorEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceivingSupervisorName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_CateringVehicleDeliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MealDefinitions",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_MealDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringContractAddendums",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AttachmentDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_CateringContractAddendums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringContractAddendums_CateringContracts_CateringContractId",
                        column: x => x.CateringContractId,
                        principalSchema: "Catering",
                        principalTable: "CateringContracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CateringSquareAllocations",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DailyScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SquareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DistributedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VarianceNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_CateringSquareAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringSquareAllocations_CateringDailySchedules_DailyScheduleId",
                        column: x => x.DailyScheduleId,
                        principalSchema: "Catering",
                        principalTable: "CateringDailySchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealComponents",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MealDefinitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ComponentName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    ComponentNameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    QuantityPerMeal = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
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
                    table.PrimaryKey("PK_MealComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealComponents_MealDefinitions_MealDefinitionId",
                        column: x => x.MealDefinitionId,
                        principalSchema: "Catering",
                        principalTable: "MealDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CateringAreas_CompanyId_Name",
                schema: "Catering",
                table: "CateringAreas",
                columns: new[] { "CompanyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringAssignments_CateringContractId_Role_EmployeeId",
                schema: "Catering",
                table: "CateringAssignments",
                columns: new[] { "CateringContractId", "Role", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringContractAddendums_CateringContractId_EffectiveFrom",
                schema: "Catering",
                table: "CateringContractAddendums",
                columns: new[] { "CateringContractId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringContracts_CompanyId_CustomerId_ServiceType",
                schema: "Catering",
                table: "CateringContracts",
                columns: new[] { "CompanyId", "CustomerId", "ServiceType" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringContracts_Number",
                schema: "Catering",
                table: "CateringContracts",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringDailySchedules_CateringContractId_ServiceDate",
                schema: "Catering",
                table: "CateringDailySchedules",
                columns: new[] { "CateringContractId", "ServiceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringSquareAllocations_DailyScheduleId_SquareId",
                schema: "Catering",
                table: "CateringSquareAllocations",
                columns: new[] { "DailyScheduleId", "SquareId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringSquares_AreaId_Code",
                schema: "Catering",
                table: "CateringSquares",
                columns: new[] { "AreaId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringVehicleDeliveries_DailyScheduleId_VehicleId",
                schema: "Catering",
                table: "CateringVehicleDeliveries",
                columns: new[] { "DailyScheduleId", "VehicleId" });

            migrationBuilder.CreateIndex(
                name: "IX_MealComponents_MealDefinitionId_ProductSkuId",
                schema: "Catering",
                table: "MealComponents",
                columns: new[] { "MealDefinitionId", "ProductSkuId" });

            migrationBuilder.CreateIndex(
                name: "IX_MealDefinitions_CompanyId_Name",
                schema: "Catering",
                table: "MealDefinitions",
                columns: new[] { "CompanyId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CateringAreas",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringAssignments",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringContractAddendums",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringSquareAllocations",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringSquares",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringVehicleDeliveries",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "MealComponents",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringContracts",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringDailySchedules",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "MealDefinitions",
                schema: "Catering");
        }
    }
}
