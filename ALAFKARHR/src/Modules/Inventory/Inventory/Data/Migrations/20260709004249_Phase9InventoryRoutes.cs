using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class Phase9InventoryRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExcludeFromPhysicalStock",
                schema: "Inventory",
                table: "WarehouseLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVirtual",
                schema: "Inventory",
                table: "WarehouseLocations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LocationUsage",
                schema: "Inventory",
                table: "WarehouseLocations",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "Internal");

            migrationBuilder.CreateTable(
                name: "InventoryOperationTypes",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OperationKind = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DefaultSourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultDestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryOperationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRouteRules",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    SourceLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRouteRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRoutes",
                schema: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ApplicationScope = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRoutes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_CompanyId_WarehouseId_LocationUsage_IsActive",
                schema: "Inventory",
                table: "WarehouseLocations",
                columns: new[] { "CompanyId", "WarehouseId", "LocationUsage", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperationTypes_CompanyId_WarehouseId_Code",
                schema: "Inventory",
                table: "InventoryOperationTypes",
                columns: new[] { "CompanyId", "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOperationTypes_CompanyId_WarehouseId_OperationKind_IsActive",
                schema: "Inventory",
                table: "InventoryOperationTypes",
                columns: new[] { "CompanyId", "WarehouseId", "OperationKind", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRouteRules_CompanyId_OperationTypeId",
                schema: "Inventory",
                table: "InventoryRouteRules",
                columns: new[] { "CompanyId", "OperationTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRouteRules_CompanyId_ProductId_ProductSkuId_ProductCategoryId",
                schema: "Inventory",
                table: "InventoryRouteRules",
                columns: new[] { "CompanyId", "ProductId", "ProductSkuId", "ProductCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRouteRules_CompanyId_RouteId_Priority",
                schema: "Inventory",
                table: "InventoryRouteRules",
                columns: new[] { "CompanyId", "RouteId", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRouteRules_CompanyId_WarehouseId_Action_DestinationLocationId_IsActive",
                schema: "Inventory",
                table: "InventoryRouteRules",
                columns: new[] { "CompanyId", "WarehouseId", "Action", "DestinationLocationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRouteRules_CompanyId_WarehouseId_Action_SourceLocationId_IsActive",
                schema: "Inventory",
                table: "InventoryRouteRules",
                columns: new[] { "CompanyId", "WarehouseId", "Action", "SourceLocationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRoutes_CompanyId_Code",
                schema: "Inventory",
                table: "InventoryRoutes",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRoutes_CompanyId_ProductId_ProductSkuId_ProductCategoryId",
                schema: "Inventory",
                table: "InventoryRoutes",
                columns: new[] { "CompanyId", "ProductId", "ProductSkuId", "ProductCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRoutes_CompanyId_WarehouseId_IsActive",
                schema: "Inventory",
                table: "InventoryRoutes",
                columns: new[] { "CompanyId", "WarehouseId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryOperationTypes",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "InventoryRouteRules",
                schema: "Inventory");

            migrationBuilder.DropTable(
                name: "InventoryRoutes",
                schema: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseLocations_CompanyId_WarehouseId_LocationUsage_IsActive",
                schema: "Inventory",
                table: "WarehouseLocations");

            migrationBuilder.DropColumn(
                name: "ExcludeFromPhysicalStock",
                schema: "Inventory",
                table: "WarehouseLocations");

            migrationBuilder.DropColumn(
                name: "IsVirtual",
                schema: "Inventory",
                table: "WarehouseLocations");

            migrationBuilder.DropColumn(
                name: "LocationUsage",
                schema: "Inventory",
                table: "WarehouseLocations");
        }
    }
}
